using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProcessingJobProcessor : IProcessingJobProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly IReadOnlyDictionary<string, IProcessingJobHandler> _handlers;
    private readonly IAuditService _auditService;
    private readonly IOrderStatusWorkflowService _orderStatusWorkflow;
    private readonly IProcessingJobWorkflowPolicy _workflowPolicy;

    public ProcessingJobProcessor(
        AppDbContext dbContext,
        IEnumerable<IProcessingJobHandler> handlers,
        IAuditService auditService,
        IOrderStatusWorkflowService orderStatusWorkflow,
        IProcessingJobWorkflowPolicy workflowPolicy)
    {
        _dbContext = dbContext;
        _handlers = handlers.ToDictionary(h => h.JobType);
        _auditService = auditService;
        _orderStatusWorkflow = orderStatusWorkflow;
        _workflowPolicy = workflowPolicy;
    }

    public async Task ProcessNextBatchAsync(CancellationToken cancellationToken)
    {
        await MarkOrdersWithFailedJobsAsync(cancellationToken);

        var jobs = await GetNextJobsToProcessAsync(DateTime.UtcNow, cancellationToken);

        foreach (var job in jobs)
        {
            await ProcessJobAsync(job, cancellationToken);
        }
    }

    private async Task<List<ProcessingJob>> GetNextJobsToProcessAsync(
        DateTime now,
        CancellationToken cancellationToken)
    {
        var queuedJobs = await _dbContext.ProcessingJobs
            .Where(j => j.Status == ProcessingJobStatus.Queued)
            .Where(j => j.NextAttemptAt == null || j.NextAttemptAt <= now)
            .Where(j => !_dbContext.ProcessingJobs.Any(f =>
                f.OrderId == j.OrderId &&
                f.Status == ProcessingJobStatus.Failed))
            .ToListAsync(cancellationToken);

        return queuedJobs
            .OrderBy(j => j.OrderId)
            .ThenBy(j => _workflowPolicy.GetProcessingPriority(j.JobType))
            .ThenBy(j => j.CreatedAt)
            .ThenBy(j => j.ProcessingJobId)
            .GroupBy(j => j.OrderId)
            .Select(g => g.First())
            .Take(5)
            .ToList();
    }

    private async Task ProcessJobAsync(
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        try
        {
            await MoveOrderIntoProcessingAsync(job.OrderId, cancellationToken);
            MarkJobAsProcessing(job);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var handler = GetHandler(job.JobType);

            await handler.HandleAsync(job, cancellationToken);

            MarkJobAsCompleted(job);
            AuditJobCompleted(job);

            await ReactivateLaterCancelledJobsAsync(job, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await HandleFailedJobAsync(job, ex, cancellationToken);
        }
    }

    private IProcessingJobHandler GetHandler(string jobType)
    {
        if (_handlers.TryGetValue(jobType, out var handler))
        {
            return handler;
        }

        throw new InvalidOperationException($"Unknown job type '{jobType}'.");
    }

    private async Task MoveOrderIntoProcessingAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var orderStatusId = await _dbContext.Orders
            .Where(o => o.OrderId == orderId)
            .Select(o => o.OrderStatusId)
            .FirstAsync(cancellationToken);

        if (orderStatusId is (int)OrderStatusEnum.Approved or (int)OrderStatusEnum.Failed)
        {
            await _orderStatusWorkflow.MoveToStatusAsync(
                orderId,
                OrderStatusEnum.InProcessing,
                "Order moved to In Processing while background fulfilment work is being completed.",
                cancellationToken);
        }
    }

    private static void MarkJobAsProcessing(ProcessingJob job)
    {
        job.Status = ProcessingJobStatus.Processing;
        job.StartedAt = DateTime.UtcNow;
        job.AttemptCount++;
        job.ErrorMessage = null;
        job.NextAttemptAt = null;
    }

    private static void MarkJobAsCompleted(ProcessingJob job)
    {
        job.Status = ProcessingJobStatus.Completed;
        job.CompletedAt = DateTime.UtcNow;
        job.FailedAt = null;
        job.ErrorMessage = null;
    }

    private async Task HandleFailedJobAsync(
        ProcessingJob job,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var safeErrorMessage = GetSafeErrorMessage(job, exception);

        job.ErrorMessage = safeErrorMessage;

        if (ShouldFailJob(job, exception))
        {
            job.Status = ProcessingJobStatus.Failed;
            job.FailedAt = DateTime.UtcNow;
            job.NextAttemptAt = null;
            job.LastRetryAt = DateTime.UtcNow;

            await _orderStatusWorkflow.MoveToStatusAsync(
                job.OrderId,
                OrderStatusEnum.Failed,
                $"Background job failed: {job.JobType}. {safeErrorMessage}",
                cancellationToken);

            await CancelLaterJobsAsync(job, safeErrorMessage, cancellationToken);
        }
        else
        {
            job.Status = ProcessingJobStatus.Queued;
            job.LastRetryAt = DateTime.UtcNow;
            job.NextAttemptAt = DateTime.UtcNow.AddMinutes(1);
        }

        AuditJobFailed(job, safeErrorMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private bool ShouldFailJob(ProcessingJob job, Exception exception)
    {
        return _workflowPolicy.IsOperatorActionRequiredFailure(job, exception) ||
            job.AttemptCount >= job.MaxAttempts;
    }

    private async Task CancelLaterJobsAsync(
        ProcessingJob failedJob,
        string failureReason,
        CancellationToken cancellationToken)
    {
        var laterJobTypes = _workflowPolicy.GetLaterJobTypes(failedJob.JobType);

        if (laterJobTypes.Count == 0)
        {
            return;
        }

        var laterJobs = await _dbContext.ProcessingJobs
            .Where(j => j.OrderId == failedJob.OrderId)
            .Where(j => j.ProcessingJobId != failedJob.ProcessingJobId)
            .Where(j => laterJobTypes.Contains(j.JobType))
            .Where(j => j.Status == ProcessingJobStatus.Queued ||
                        j.Status == ProcessingJobStatus.Processing)
            .ToListAsync(cancellationToken);

        foreach (var laterJob in laterJobs)
        {
            var oldStatus = laterJob.Status;

            laterJob.Status = ProcessingJobStatus.Cancelled;
            laterJob.ErrorMessage = $"Cancelled because required job {failedJob.JobType} failed: {failureReason}";
            laterJob.NextAttemptAt = null;

            _auditService.AddSystemAction(
                "ProcessingJob",
                laterJob.ProcessingJobId,
                "Cancelled",
                new { Status = oldStatus },
                new
                {
                    Status = ProcessingJobStatus.Cancelled,
                    laterJob.JobType,
                    BlockedByJobType = failedJob.JobType
                },
                $"Background job cancelled because required job {failedJob.JobType} failed.");
        }
    }

    private async Task ReactivateLaterCancelledJobsAsync(
        ProcessingJob completedJob,
        CancellationToken cancellationToken)
    {
        var laterJobTypes = _workflowPolicy.GetLaterJobTypes(completedJob.JobType);

        if (laterJobTypes.Count == 0)
        {
            return;
        }

        var laterJobs = await _dbContext.ProcessingJobs
            .Where(j => j.OrderId == completedJob.OrderId)
            .Where(j => laterJobTypes.Contains(j.JobType))
            .Where(j => j.Status == ProcessingJobStatus.Cancelled)
            .ToListAsync(cancellationToken);

        foreach (var laterJob in laterJobs)
        {
            var oldStatus = laterJob.Status;

            laterJob.Status = ProcessingJobStatus.Queued;
            laterJob.ErrorMessage = null;
            laterJob.NextAttemptAt = null;

            _auditService.AddSystemAction(
                "ProcessingJob",
                laterJob.ProcessingJobId,
                "Requeued",
                new { Status = oldStatus },
                new
                {
                    Status = ProcessingJobStatus.Queued,
                    laterJob.JobType,
                    ResumedAfterJobType = completedJob.JobType
                },
                $"Background job requeued after required job {completedJob.JobType} completed.");
        }
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
            var safeErrorMessage = GetSafeStoredErrorMessage(job);

            await _orderStatusWorkflow.MoveToStatusAsync(
                job.OrderId,
                OrderStatusEnum.Failed,
                $"Background job failed: {job.JobType}. {safeErrorMessage}",
                cancellationToken);

            await CancelLaterJobsAsync(job, safeErrorMessage, cancellationToken);
        }

        if (failedJobs.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private void AuditJobCompleted(ProcessingJob job)
    {
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
            $"Background job completed: {job.JobType}.");
    }

    private void AuditJobFailed(ProcessingJob job, string safeErrorMessage)
    {
        _auditService.AddSystemAction(
            "ProcessingJob",
            job.ProcessingJobId,
            job.Status == ProcessingJobStatus.Failed ? "Failed" : "RetryQueued",
            new { Status = ProcessingJobStatus.Processing },
            new
            {
                job.Status,
                job.AttemptCount,
                Error = safeErrorMessage
            },
            $"Background job {job.JobType} failed: {safeErrorMessage}");
    }

    private static string GetSafeErrorMessage(ProcessingJob job, Exception exception)
    {
        if (exception is OperatorActionRequiredException)
        {
            return exception.Message;
        }

        if (job.JobType == ProcessingJobType.GenerateSdsBundle &&
            exception is IOException or UnauthorizedAccessException)
        {
            return "SDS bundle generation failed because one or more product SDS PDF files could not be read. Regenerate the product SDS and retry the job.";
        }

        return GetSafeStoredErrorMessage(exception.Message);
    }

    private static string GetSafeStoredErrorMessage(ProcessingJob job)
    {
        if (job.JobType == ProcessingJobType.GenerateSdsBundle &&
            ContainsFilePath(job.ErrorMessage))
        {
            return "SDS bundle generation failed because one or more product SDS PDF files could not be read. Regenerate the product SDS and retry the job.";
        }

        return GetSafeStoredErrorMessage(job.ErrorMessage);
    }

    private static string GetSafeStoredErrorMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "The background job failed.";
        }

        return ContainsFilePath(message)
            ? "The background job failed because a required file could not be read."
            : message;
    }

    private static bool ContainsFilePath(string? message)
    {
        return !string.IsNullOrWhiteSpace(message) &&
            (message.Contains(@":\", StringComparison.OrdinalIgnoreCase) ||
             message.Contains(@":/", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("/documents/", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("\\documents\\", StringComparison.OrdinalIgnoreCase));
    }
}
