using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class JobProcessingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<JobProcessingService> _logger;

    public JobProcessingService(
        IServiceScopeFactory scopeFactory,
        ILogger<JobProcessingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Job processing service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;

                var jobs = await dbContext.ProcessingJobs
                    .Where(j => j.Status == "Queued")
                    .Where(j => j.NextAttemptAt == null || j.NextAttemptAt <= now)
                    .OrderBy(j => j.CreatedAt)
                    .Take(5)
                    .ToListAsync(stoppingToken);

                foreach (var job in jobs)
                {
                    await ProcessJobAsync(dbContext, job, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing background jobs.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessJobAsync(
        AppDbContext dbContext,
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        try
        {
            job.Status = "Processing";
            job.StartedAt = DateTime.UtcNow;
            job.AttemptCount++;
            job.ErrorMessage = null;

            await dbContext.SaveChangesAsync(cancellationToken);

            switch (job.JobType)
            {
                case "GenerateOrderSummaryDocument":
                    await GenerateDocumentAsync(dbContext, job, "OrderSummary", cancellationToken);
                    break;

                case "GenerateSdsBundle":
                    await GenerateDocumentAsync(dbContext, job, "SafetyDataSheetBundle", cancellationToken);
                    break;

                case "GenerateDeliveryNote":
                    await GenerateDocumentAsync(dbContext, job, "DeliveryNote", cancellationToken);
                    break;

                case "CreateSubmissionNotification":
                    await CreateNotificationAsync(dbContext, job, "OrderSubmitted", cancellationToken);
                    break;

                case "CreateApprovalNotification":
                    await CreateNotificationAsync(dbContext, job, "OrderApproved", cancellationToken);
                    break;

                case "PushToLogisticsProvider":
                    await PushToLogisticsProviderAsync(dbContext, job, cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown job type '{job.JobType}'.");
            }

            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.FailedAt = null;
            job.ErrorMessage = null;

            AddAuditLog(
                dbContext,
                "ProcessingJob",
                job.ProcessingJobId,
                "Completed",
                null,
                null,
                $$"""{"status":"Completed","jobType":"{{job.JobType}}"}""",
                $"Background job completed: {job.JobType}."
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            job.ErrorMessage = ex.Message;

            if (job.AttemptCount >= job.MaxAttempts)
            {
                job.Status = "Failed";
                job.FailedAt = DateTime.UtcNow;
                job.NextAttemptAt = null;

                var order = await dbContext.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == job.OrderId, cancellationToken);

                if (order != null)
                {
                    var oldOrderStatus = order.OrderStatusId;

                    order.OrderStatusId = 8; // Failed
                    order.FailureReason = $"Background job failed: {job.JobType}. {ex.Message}";
                    order.UpdatedAt = DateTime.UtcNow;

                    AddAuditLog(
                        dbContext,
                        "Order",
                        order.OrderId,
                        "StatusChanged:Failed",
                        null,
                        $$"""{"statusId":{{oldOrderStatus}}}""",
                        $$"""{"statusId":8,"status":"Failed","reason":"{{EscapeJson(order.FailureReason)}}"}""",
                        $"Order moved to Failed after background job reached max retry attempts."
                    );
                }
            }
            else
            {
                job.Status = "Queued";
                job.LastRetryAt = DateTime.UtcNow;
                job.NextAttemptAt = DateTime.UtcNow.AddMinutes(1);
            }

            AddAuditLog(
                dbContext,
                "ProcessingJob",
                job.ProcessingJobId,
                job.Status == "Failed" ? "Failed" : "RetryQueued",
                null,
                $$"""{"status":"Processing"}""",
                $$"""{"status":"{{job.Status}}","attemptCount":{{job.AttemptCount}},"error":"{{EscapeJson(ex.Message)}}"}""",
                $"Background job {job.JobType} failed: {ex.Message}"
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task GenerateDocumentAsync(
      AppDbContext dbContext,
      ProcessingJob job,
      string documentType,
      CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        var safeOrderNumber = order.OrderNumber.ToLowerInvariant();
        var safeDocumentType = documentType.ToLowerInvariant();

        var document = new Document
        {
            OrderId = order.OrderId,
            DocumentType = documentType,
            FileName = $"{safeOrderNumber}_{safeDocumentType}.pdf",
            FilePath = $"/documents/{safeOrderNumber}_{safeDocumentType}.pdf",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = null
        };

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        AddAuditLog(
            dbContext,
            "Document",
            document.DocumentId,
            "Generated",
            null,
            null,
            $$"""{"documentId":{{document.DocumentId}},"orderId":{{order.OrderId}},"documentType":"{{documentType}}","orderNumber":"{{order.OrderNumber}}"}""",
            $"{documentType} generated by background job."
        );
    }

    private static async Task CreateNotificationAsync(
    AppDbContext dbContext,
    ProcessingJob job,
    string notificationType,
    CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .Include(o => o.Customer)
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        var recipientEmail = $"purchasing{order.CustomerId}@simulated-customer.co.uk";

        var notification = new Notification
        {
            OrderId = order.OrderId,
            RecipientEmail = recipientEmail,
            NotificationType = notificationType,
            Subject = $"Confirmation for {order.OrderNumber}",
            CreatedAt = DateTime.UtcNow,
            SentAt = DateTime.UtcNow,
            Status = "Sent",
            FailureReason = null
        };

        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);

        AddAuditLog(
            dbContext,
            "Notification",
            notification.NotificationId,
            "Sent",
            null,
            null,
            $$"""{"notificationId":{{notification.NotificationId}},"orderId":{{order.OrderId}},"notificationType":"{{notificationType}}","recipientEmail":"{{recipientEmail}}"}""",
            $"{notificationType} notification simulated by background job."
        );
    }

    private static async Task PushToLogisticsProviderAsync(
    AppDbContext dbContext,
    ProcessingJob job,
    CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        var oldStatus = order.OrderStatusId;

        order.OrderStatusId = 5; // In Processing
        order.UpdatedAt = DateTime.UtcNow;

        AddAuditLog(
            dbContext,
            "Order",
            order.OrderId,
            "StatusChanged:In Processing",
            null,
            $$"""{"statusId":{{oldStatus}}}""",
            $$"""{"statusId":5,"status":"In Processing"}""",
            "Order moved to In Processing while background fulfilment work is being completed."
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        var processingStatus = order.OrderStatusId;

        order.OrderStatusId = 6; // Awaiting Dispatch
        order.UpdatedAt = DateTime.UtcNow;

        AddAuditLog(
            dbContext,
            "Order",
            order.OrderId,
            "StatusChanged:Awaiting Dispatch",
            null,
            $$"""{"statusId":{{processingStatus}}}""",
            $$"""{"statusId":6,"status":"Awaiting Dispatch"}""",
            "Order moved to Awaiting Dispatch after simulated logistics provider push."
        );
    }

    private static void AddAuditLog(
        AppDbContext dbContext,
        string entityType,
        int entityId,
        string action,
        int? performedByUserId,
        string? oldValuesJson,
        string? newValuesJson,
        string notes)
    {
        dbContext.AuditLogs.Add(new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = performedByUserId,
            PerformedAt = DateTime.UtcNow,
            OldValuesJson = oldValuesJson,
            NewValuesJson = newValuesJson,
            Notes = notes
        });
    }

    private static string EscapeJson(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}