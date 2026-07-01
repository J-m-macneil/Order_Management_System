using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProcessingJobProcessor : IProcessingJobProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly IOrderDocumentService _documentService;
    private readonly IAuditService _auditService;

    public ProcessingJobProcessor(
        AppDbContext dbContext,
        IOrderDocumentService documentService,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _documentService = documentService;
        _auditService = auditService;
    }

    public async Task ProcessNextBatchAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await MarkOrdersWithFailedJobsAsync(cancellationToken);
        await EnsureApprovalWorkflowJobsAsync(now, cancellationToken);

        var jobs = await _dbContext.ProcessingJobs
            .Where(j => j.Status == ProcessingJobStatus.Queued)
            .Where(j => j.NextAttemptAt == null || j.NextAttemptAt <= now)
            .OrderBy(j => j.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var job in jobs)
        {
            await ProcessJobAsync(job, cancellationToken);
        }
    }

    private async Task ProcessJobAsync(
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        try
        {
            if (job.JobType == ProcessingJobType.PushToLogisticsProvider &&
                !await _documentService.RequiredApprovalDocumentsExistAsync(job.OrderId, cancellationToken))
            {
                job.NextAttemptAt = DateTime.UtcNow.AddSeconds(15);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            job.Status = ProcessingJobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;
            job.AttemptCount++;
            job.ErrorMessage = null;

            await _dbContext.SaveChangesAsync(cancellationToken);

            switch (job.JobType)
            {
                case ProcessingJobType.GenerateOrderSummaryDocument:
                case ProcessingJobType.GenerateSdsBundle:
                    await _documentService.GenerateForJobAsync(job, cancellationToken);
                    break;

                case ProcessingJobType.CreateSubmissionNotification:
                    await CreateNotificationAsync(job, "OrderSubmitted", cancellationToken);
                    break;

                case ProcessingJobType.CreateApprovalNotification:
                    await CreateNotificationAsync(job, "OrderApproved", cancellationToken);
                    break;

                case ProcessingJobType.PushToLogisticsProvider:
                    await PushToLogisticsProviderAsync(job, cancellationToken);
                    break;

                case ProcessingJobType.ProcessLogisticsEvent:
                    await ProcessLogisticsEventAsync(job, cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown job type '{job.JobType}'.");
            }

            job.Status = ProcessingJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.FailedAt = null;
            job.ErrorMessage = null;

            _auditService.AddSystemAction(
                "ProcessingJob",
                job.ProcessingJobId,
                "Completed",
                null,
                new
                {
                    Status = ProcessingJobStatus.Completed,
                    job.JobType
                },
                $"Background job completed: {job.JobType}."
            );

            await ResumeFailedOrderAfterSuccessfulJobAsync(job, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await HandleFailedJobAsync(job, ex, cancellationToken);
        }
    }

    private async Task HandleFailedJobAsync(
        ProcessingJob job,
        Exception exception,
        CancellationToken cancellationToken)
    {
        job.ErrorMessage = exception.Message;

        if (job.AttemptCount >= job.MaxAttempts)
        {
            job.Status = ProcessingJobStatus.Failed;
            job.FailedAt = DateTime.UtcNow;
            job.NextAttemptAt = null;

            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == job.OrderId, cancellationToken);

            if (order != null)
            {
                order.FailureReason = $"Background job failed: {job.JobType}. {exception.Message}";

                MoveOrderStatus(
                    order,
                    OrderStatusEnum.Failed,
                    order.FailureReason);
            }
        }
        else
        {
            job.Status = ProcessingJobStatus.Queued;
            job.LastRetryAt = DateTime.UtcNow;
            job.NextAttemptAt = DateTime.UtcNow.AddMinutes(1);
        }

        _auditService.AddSystemAction(
            "ProcessingJob",
            job.ProcessingJobId,
            job.Status == ProcessingJobStatus.Failed ? "Failed" : "RetryQueued",
            new { Status = ProcessingJobStatus.Processing },
            new
            {
                job.Status,
                job.AttemptCount,
                Error = exception.Message
            },
            $"Background job {job.JobType} failed: {exception.Message}"
        );

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task MarkOrdersWithFailedJobsAsync(CancellationToken cancellationToken)
    {
        var failedJobs = await _dbContext.ProcessingJobs
            .Include(j => j.Order)
            .Where(j => j.Status == ProcessingJobStatus.Failed)
            .Where(j => j.Order.OrderStatusId != (int)OrderStatusEnum.Failed)
            .ToListAsync(cancellationToken);

        foreach (var job in failedJobs)
        {
            var order = job.Order;

            order.FailureReason = $"Background job failed: {job.JobType}. {job.ErrorMessage}";

            MoveOrderStatus(
                order,
                OrderStatusEnum.Failed,
                order.FailureReason);
        }

        if (failedJobs.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task EnsureApprovalWorkflowJobsAsync(
        DateTime now,
        CancellationToken cancellationToken)
    {
        var activeStatuses = new[] { (int)OrderStatusEnum.Approved, (int)OrderStatusEnum.InProcessing };
        var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .Where(o => activeStatuses.Contains(o.OrderStatusId))
            .ToListAsync(cancellationToken);

        foreach (var order in orders)
        {
            var missingDocumentTypes = await _documentService.GetMissingApprovalDocumentTypesAsync(order, cancellationToken);

            foreach (var documentType in missingDocumentTypes)
            {
                var jobType = _documentService.GetGenerationJobType(documentType);

                if (!await HasActiveOrCompletedJobAsync(order.OrderId, jobType, cancellationToken))
                {
                    _dbContext.ProcessingJobs.Add(CreateRecoveryJob(order.OrderId, jobType, now));
                }
            }

            if (!await HasActiveOrCompletedJobAsync(order.OrderId, ProcessingJobType.PushToLogisticsProvider, cancellationToken))
            {
                _dbContext.ProcessingJobs.Add(CreateRecoveryJob(order.OrderId, ProcessingJobType.PushToLogisticsProvider, now));
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> HasActiveOrCompletedJobAsync(
        int orderId,
        string jobType,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ProcessingJobs.AnyAsync(j =>
            j.OrderId == orderId &&
            j.JobType == jobType &&
            (j.Status == ProcessingJobStatus.Queued ||
             j.Status == ProcessingJobStatus.Processing ||
             j.Status == ProcessingJobStatus.Completed),
            cancellationToken);
    }

    private static ProcessingJob CreateRecoveryJob(int orderId, string jobType, DateTime now)
    {
        return new ProcessingJob
        {
            OrderId = orderId,
            JobType = jobType,
            Status = ProcessingJobStatus.Queued,
            AttemptCount = 0,
            MaxAttempts = 3,
            CreatedAt = now
        };
    }

    private async Task CreateNotificationAsync(
        ProcessingJob job,
        string notificationType,
        CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
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

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _auditService.AddSystemAction(
            "Notification",
            notification.NotificationId,
            "Sent",
            null,
            new
            {
                notification.NotificationId,
                order.OrderId,
                NotificationType = notificationType,
                RecipientEmail = recipientEmail
            },
            $"{notificationType} notification simulated by background job."
        );
    }

    private async Task ResumeFailedOrderAfterSuccessfulJobAsync(
        ProcessingJob completedJob,
        CancellationToken cancellationToken)
    {
        if (completedJob.JobType == ProcessingJobType.PushToLogisticsProvider)
            return;

        var order = await _dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderId == completedJob.OrderId, cancellationToken);

        if (order == null || order.OrderStatusId != (int)OrderStatusEnum.Failed)
            return;

        var hasOtherFailedJobs = await _dbContext.ProcessingJobs.AnyAsync(j =>
            j.OrderId == completedJob.OrderId &&
            j.ProcessingJobId != completedJob.ProcessingJobId &&
            j.Status == ProcessingJobStatus.Failed,
            cancellationToken);

        if (hasOtherFailedJobs || !await _documentService.RequiredApprovalDocumentsExistAsync(order.OrderId, cancellationToken))
            return;

        if (await HasCompletedJobAsync(order.OrderId, ProcessingJobType.PushToLogisticsProvider, cancellationToken))
        {
            MoveOrderStatus(
                order,
                OrderStatusEnum.AwaitingDispatch,
                "Order recovered to Awaiting Dispatch after failed background work was retried successfully.");

            return;
        }

        if (!await HasActiveOrCompletedJobAsync(order.OrderId, ProcessingJobType.PushToLogisticsProvider, cancellationToken))
        {
            _dbContext.ProcessingJobs.Add(CreateRecoveryJob(order.OrderId, ProcessingJobType.PushToLogisticsProvider, DateTime.UtcNow));
        }

        MoveOrderStatus(
            order,
            OrderStatusEnum.InProcessing,
            "Order recovered to In Processing after failed background work was retried successfully.");
    }

    private async Task PushToLogisticsProviderAsync(
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        MoveOrderStatus(
            order,
            OrderStatusEnum.InProcessing,
            "Order moved to In Processing while background fulfilment work is being completed.");

        await _dbContext.SaveChangesAsync(cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        MoveOrderStatus(
            order,
            OrderStatusEnum.AwaitingDispatch,
            "Order moved to Awaiting Dispatch after simulated logistics provider push.");
    }

    private async Task ProcessLogisticsEventAsync(
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .FirstAsync(o => o.OrderId == job.OrderId, cancellationToken);

        if (job.PayloadJson?.Contains("DELIVERED", StringComparison.OrdinalIgnoreCase) == true &&
            order.OrderStatusId == (int)OrderStatusEnum.AwaitingDispatch)
        {
            MoveOrderStatus(
                order,
                OrderStatusEnum.Completed,
                "Order completed after simulated delivered logistics event.");
        }
    }

    private async Task<bool> HasCompletedJobAsync(
        int orderId,
        string jobType,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ProcessingJobs.AnyAsync(j =>
            j.OrderId == orderId &&
            j.JobType == jobType &&
            j.Status == ProcessingJobStatus.Completed,
            cancellationToken);
    }

    private void MoveOrderStatus(
        Domain.Entities.Orders.Order order,
        OrderStatusEnum toStatus,
        string reason)
    {
        var oldStatusId = order.OrderStatusId;
        var toStatusId = (int)toStatus;

        if (oldStatusId == toStatusId)
            return;

        order.OrderStatusId = toStatusId;
        order.UpdatedAt = DateTime.UtcNow;

        AddStatusHistory(
            order.OrderId,
            oldStatusId,
            toStatusId,
            order.CreatedByUserId,
            reason);

        _auditService.AddSystemAction(
            "Order",
            order.OrderId,
            $"StatusChanged:{GetStatusName(toStatus)}",
            new { StatusId = oldStatusId },
            new
            {
                StatusId = toStatusId,
                Status = GetStatusName(toStatus)
            },
            reason
        );
    }

    private static string GetStatusName(OrderStatusEnum status)
    {
        return status switch
        {
            OrderStatusEnum.InProcessing => "In Processing",
            OrderStatusEnum.AwaitingDispatch => "Awaiting Dispatch",
            OrderStatusEnum.Completed => "Completed",
            OrderStatusEnum.Failed => "Failed",
            _ => status.ToString()
        };
    }

    private void AddStatusHistory(
        int orderId,
        int fromStatusId,
        int toStatusId,
        int changedByUserId,
        string? reason)
    {
        _dbContext.OrderStatusHistories.Add(new Domain.Entities.Status.OrderStatusHistory
        {
            OrderId = orderId,
            FromStatusId = fromStatusId,
            ToStatusId = toStatusId,
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTime.UtcNow,
            Reason = reason
        });
    }
}
