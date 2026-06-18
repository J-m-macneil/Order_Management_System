using Application.Interfaces;
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
                var documentGenerator = scope.ServiceProvider.GetRequiredService<IOrderDocumentGenerator>();

                var now = DateTime.UtcNow;

                await MarkOrdersWithFailedJobsAsync(dbContext, stoppingToken);
                await EnsureApprovalWorkflowJobsAsync(dbContext, now, stoppingToken);

                var jobs = await dbContext.ProcessingJobs
                    .Where(j => j.Status == "Queued")
                    .Where(j => j.NextAttemptAt == null || j.NextAttemptAt <= now)
                    .OrderBy(j => j.CreatedAt)
                    .Take(5)
                    .ToListAsync(stoppingToken);

                foreach (var job in jobs)
                {
                    await ProcessJobAsync(dbContext, documentGenerator, job, stoppingToken);
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
        IOrderDocumentGenerator documentGenerator,
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        try
        {
            if (job.JobType == "PushToLogisticsProvider" &&
                !await RequiredApprovalDocumentsExistAsync(dbContext, job.OrderId, cancellationToken))
            {
                job.NextAttemptAt = DateTime.UtcNow.AddSeconds(15);
                await dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            job.Status = "Processing";
            job.StartedAt = DateTime.UtcNow;
            job.AttemptCount++;
            job.ErrorMessage = null;

            await dbContext.SaveChangesAsync(cancellationToken);

            switch (job.JobType)
            {
                case "GenerateOrderSummaryDocument":
                    await GenerateDocumentAsync(documentGenerator, dbContext, job, "OrderSummary", cancellationToken);
                    break;

                case "GenerateSdsBundle":
                    await GenerateDocumentAsync(documentGenerator, dbContext, job, "SafetyDataSheetBundle", cancellationToken);
                    break;

                case "GenerateDeliveryNote":
                    await GenerateDocumentAsync(documentGenerator, dbContext, job, "DeliveryNote", cancellationToken);
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

                case "ProcessLogisticsEvent":
                    await ProcessLogisticsEventAsync(dbContext, job, cancellationToken);
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

            await ResumeFailedOrderAfterSuccessfulJobAsync(dbContext, job, cancellationToken);

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

                    AddStatusHistory(
                        dbContext,
                        order.OrderId,
                        oldOrderStatus,
                        8,
                        order.CreatedByUserId,
                        order.FailureReason);

                    AddAuditLog(
                        dbContext,
                        "Order",
                        order.OrderId,
                        "StatusChanged:Failed",
                        null,
                        $$"""{"statusId":{{oldOrderStatus}}}""",
                        $$"""{"statusId":8,"status":"Failed","reason":"{{EscapeJson(order.FailureReason)}}"}""",
                        "Order moved to Failed after background job reached max retry attempts."
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
        IOrderDocumentGenerator generator,
        AppDbContext dbContext,
        ProcessingJob job,
        string documentType,
        CancellationToken cancellationToken)
    {
        var document = await generator.GenerateAsync(
            job.OrderId,
            documentType,
            cancellationToken);

        AddAuditLog(
            dbContext,
            "Document",
            document.DocumentId,
            "Generated",
            null,
            null,
            $$"""{"documentId":{{document.DocumentId}},"orderId":{{job.OrderId}},"documentType":"{{documentType}}"}""",
            $"{documentType} generated as PDF by background job."
        );
    }

    private static async Task MarkOrdersWithFailedJobsAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var failedJobs = await dbContext.ProcessingJobs
            .Include(j => j.Order)
            .Where(j => j.Status == "Failed")
            .Where(j => j.Order.OrderStatusId != 8)
            .ToListAsync(cancellationToken);

        foreach (var job in failedJobs)
        {
            var order = job.Order;

            order.FailureReason = $"Background job failed: {job.JobType}. {job.ErrorMessage}";

            MoveOrderStatus(
                dbContext,
                order,
                8,
                order.FailureReason);
        }

        if (failedJobs.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task EnsureApprovalWorkflowJobsAsync(
        AppDbContext dbContext,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var activeStatuses = new[] { 4, 5 };
        var orders = await dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .Where(o => activeStatuses.Contains(o.OrderStatusId))
            .ToListAsync(cancellationToken);

        foreach (var order in orders)
        {
            var requiredDocumentTypes = new List<string> { "DeliveryNote" };

            if (order.OrderItems.Any(i =>
                i.DeletedAt == null &&
                (i.Product.RequiresSds || i.Product.IsRestricted)))
            {
                requiredDocumentTypes.Add("SafetyDataSheetBundle");
            }

            var generatedDocumentTypes = await dbContext.Documents
                .Where(d => d.OrderId == order.OrderId)
                .Where(d => requiredDocumentTypes.Contains(d.DocumentType))
                .Select(d => d.DocumentType)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var documentType in requiredDocumentTypes.Except(generatedDocumentTypes))
            {
                var jobType = documentType == "SafetyDataSheetBundle"
                    ? "GenerateSdsBundle"
                    : "GenerateDeliveryNote";

                if (!await HasActiveOrCompletedJobAsync(dbContext, order.OrderId, jobType, cancellationToken))
                {
                    dbContext.ProcessingJobs.Add(CreateRecoveryJob(order.OrderId, jobType, now));
                }
            }

            if (!await HasActiveOrCompletedJobAsync(dbContext, order.OrderId, "PushToLogisticsProvider", cancellationToken))
            {
                dbContext.ProcessingJobs.Add(CreateRecoveryJob(order.OrderId, "PushToLogisticsProvider", now));
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<bool> HasActiveOrCompletedJobAsync(
        AppDbContext dbContext,
        int orderId,
        string jobType,
        CancellationToken cancellationToken)
    {
        return await dbContext.ProcessingJobs.AnyAsync(j =>
            j.OrderId == orderId &&
            j.JobType == jobType &&
            (j.Status == "Queued" || j.Status == "Processing" || j.Status == "Completed"),
            cancellationToken);
    }

    private static ProcessingJob CreateRecoveryJob(int orderId, string jobType, DateTime now)
    {
        return new ProcessingJob
        {
            OrderId = orderId,
            JobType = jobType,
            Status = "Queued",
            AttemptCount = 0,
            MaxAttempts = 3,
            CreatedAt = now
        };
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
            Status = "Sent"
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

    private static async Task<bool> RequiredApprovalDocumentsExistAsync(
        AppDbContext dbContext,
        int orderId,
        CancellationToken cancellationToken)
    {
        var requiredDocumentTypes = new List<string> { "DeliveryNote" };

        var requiresSdsBundle = await dbContext.OrderItems
            .Where(i => i.OrderId == orderId && i.DeletedAt == null)
            .AnyAsync(i => i.Product.RequiresSds || i.Product.IsRestricted, cancellationToken);

        if (requiresSdsBundle)
        {
            requiredDocumentTypes.Add("SafetyDataSheetBundle");
        }

        var generatedDocumentTypes = await dbContext.Documents
            .Where(d => d.OrderId == orderId)
            .Where(d => requiredDocumentTypes.Contains(d.DocumentType))
            .Select(d => d.DocumentType)
            .Distinct()
            .ToListAsync(cancellationToken);

        return requiredDocumentTypes.All(generatedDocumentTypes.Contains);
    }

    private static async Task ResumeFailedOrderAfterSuccessfulJobAsync(
        AppDbContext dbContext,
        ProcessingJob completedJob,
        CancellationToken cancellationToken)
    {
        if (completedJob.JobType == "PushToLogisticsProvider")
            return;

        var order = await dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderId == completedJob.OrderId, cancellationToken);

        if (order == null || order.OrderStatusId != 8)
            return;

        var hasOtherFailedJobs = await dbContext.ProcessingJobs.AnyAsync(j =>
            j.OrderId == completedJob.OrderId &&
            j.ProcessingJobId != completedJob.ProcessingJobId &&
            j.Status == "Failed",
            cancellationToken);

        if (hasOtherFailedJobs || !await RequiredApprovalDocumentsExistAsync(dbContext, order.OrderId, cancellationToken))
            return;

        if (await HasCompletedJobAsync(dbContext, order.OrderId, "PushToLogisticsProvider", cancellationToken))
        {
            MoveOrderStatus(
                dbContext,
                order,
                6,
                "Order recovered to Awaiting Dispatch after failed background work was retried successfully.");

            return;
        }

        if (!await HasActiveOrCompletedJobAsync(dbContext, order.OrderId, "PushToLogisticsProvider", cancellationToken))
        {
            dbContext.ProcessingJobs.Add(CreateRecoveryJob(order.OrderId, "PushToLogisticsProvider", DateTime.UtcNow));
        }

        MoveOrderStatus(
            dbContext,
            order,
            5,
            "Order recovered to In Processing after failed background work was retried successfully.");
    }

    private static async Task PushToLogisticsProviderAsync(
        AppDbContext dbContext,
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        MoveOrderStatus(
            dbContext,
            order,
            5,
            "Order moved to In Processing while background fulfilment work is being completed.");

        await dbContext.SaveChangesAsync(cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        MoveOrderStatus(
            dbContext,
            order,
            6,
            "Order moved to Awaiting Dispatch after simulated logistics provider push.");
    }

    private static async Task ProcessLogisticsEventAsync(
        AppDbContext dbContext,
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        if (job.PayloadJson?.Contains("DELIVERED", StringComparison.OrdinalIgnoreCase) == true &&
            order.OrderStatusId == 6)
        {
            MoveOrderStatus(
                dbContext,
                order,
                7,
                "Order completed after simulated delivered logistics event.");
        }
    }

    private static async Task<bool> HasCompletedJobAsync(
        AppDbContext dbContext,
        int orderId,
        string jobType,
        CancellationToken cancellationToken)
    {
        return await dbContext.ProcessingJobs.AnyAsync(j =>
            j.OrderId == orderId &&
            j.JobType == jobType &&
            j.Status == "Completed",
            cancellationToken);
    }

    private static void MoveOrderStatus(
        AppDbContext dbContext,
        Domain.Entities.Orders.Order order,
        int toStatusId,
        string reason)
    {
        var oldStatusId = order.OrderStatusId;

        if (oldStatusId == toStatusId)
            return;

        order.OrderStatusId = toStatusId;
        order.UpdatedAt = DateTime.UtcNow;

        AddStatusHistory(
            dbContext,
            order.OrderId,
            oldStatusId,
            toStatusId,
            order.CreatedByUserId,
            reason);

        AddAuditLog(
            dbContext,
            "Order",
            order.OrderId,
            $"StatusChanged:{GetStatusName(toStatusId)}",
            null,
            $$"""{"statusId":{{oldStatusId}}}""",
            $$"""{"statusId":{{toStatusId}},"status":"{{GetStatusName(toStatusId)}}"}""",
            reason
        );
    }

    private static string GetStatusName(int statusId)
    {
        return statusId switch
        {
            5 => "In Processing",
            6 => "Awaiting Dispatch",
            7 => "Completed",
            8 => "Failed",
            _ => $"Status {statusId}"
        };
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

    private static void AddStatusHistory(
        AppDbContext dbContext,
        int orderId,
        int fromStatusId,
        int toStatusId,
        int changedByUserId,
        string? reason)
    {
        dbContext.OrderStatusHistories.Add(new Domain.Entities.Status.OrderStatusHistory
        {
            OrderId = orderId,
            FromStatusId = fromStatusId,
            ToStatusId = toStatusId,
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTime.UtcNow,
            Reason = reason
        });
    }

    private static string EscapeJson(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
