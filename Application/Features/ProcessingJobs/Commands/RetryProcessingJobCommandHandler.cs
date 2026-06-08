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

        if (job.Status != "Failed")
            throw new Exception("Only failed jobs can be retried.");

        var oldValues = CreateSnapshot(job);

        job.Status = "Queued";
        job.NextAttemptAt = DateTime.UtcNow;
        job.ErrorMessage = null;
        job.FailedAt = null;
        job.AttemptCount = 0;

        await _repo.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "ProcessingJob",
            job.ProcessingJobId,
            "RetryQueued",
            oldValues,
            CreateSnapshot(job),
            $"Processing job retry queued: {job.JobType} for order #{job.OrderId}.",
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
