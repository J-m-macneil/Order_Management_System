using Domain.Repositories;
using MediatR;

namespace Application.Features.ProcessingJobs.Commands.RetryProcessingJob;

public class RetryProcessingJobCommandHandler : IRequestHandler<RetryProcessingJobCommand>
{
    private readonly IProcessingJobRepository _repo;

    public RetryProcessingJobCommandHandler(IProcessingJobRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(RetryProcessingJobCommand request, CancellationToken ct)
    {
        var job = await _repo.GetByIdAsync(request.Id, ct);

        if (job == null)
            throw new Exception("Job not found");

        if (job.Status != "Failed")
            throw new Exception("Only failed jobs can be retried.");

        job.Status = "Queued";
        job.NextAttemptAt = DateTime.UtcNow;
        job.ErrorMessage = null;
        job.FailedAt = null;
        job.AttemptCount = 0;

        await _repo.SaveChangesAsync(ct);
    }
}