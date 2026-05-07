using Domain.Entities;

namespace Domain.Repositories;

public interface IProcessingJobRepository
{
    Task<List<ProcessingJob>> GetFailedJobsAsync(CancellationToken ct);
    Task<List<ProcessingJob>> GetByOrderIdAsync(int orderId, CancellationToken ct);
    Task<ProcessingJob?> GetByIdAsync(int id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}