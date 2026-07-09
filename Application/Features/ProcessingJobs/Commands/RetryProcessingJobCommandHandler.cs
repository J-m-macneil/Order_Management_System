using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.ProcessingJobs.Commands.RetryProcessingJob;

public class RetryProcessingJobCommandHandler : IRequestHandler<RetryProcessingJobCommand>
{
    private readonly IProcessingJobRepository _repo;
    private readonly IAuditService _audit;

    public RetryProcessingJobCommandHandler(
        IProcessingJobRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task Handle(RetryProcessingJobCommand request, CancellationToken ct)
    {
        var job = await _repo.GetByIdAsync(request.Id, ct);

        if (job == null)
            throw new Exception("Job not found");

        if (job.Status != ProcessingJobStatus.Failed)
            throw new Exception("Only failed jobs can be retried.");

        if (job.AttemptCount >= job.MaxAttempts)
            throw new Exception("This processing job has reached the retry limit.");

        var oldValues = CreateSnapshot(job);

        job.Status = ProcessingJobStatus.Queued;
        job.NextAttemptAt = DateTime.UtcNow;
        job.LastRetryAt = DateTime.UtcNow;
        job.ErrorMessage = null;
        job.FailedAt = null;

        await _repo.SaveChangesAsync(ct);

        var newValues = CreateSnapshot(job);

        await _audit.LogAsync(
            "ProcessingJob",
            job.ProcessingJobId,
            "RetryQueued",
            oldValues,
            newValues,
            $"Processing job retry queued: {job.JobType}.",
            ct);
    }

    private static object CreateSnapshot(ProcessingJob job)
    {
        return new
        {
            job.ProcessingJobId,
            job.OrderId,
            job.JobType,
            job.Status,
            job.AttemptCount,
            job.MaxAttempts,
            job.StartedAt,
            job.CompletedAt,
            job.FailedAt,
            job.ErrorMessage,
            job.LastRetryAt,
            job.NextAttemptAt,
            job.PayloadJson
        };
    }
}
