using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProcessingJobRepository : IProcessingJobRepository
{
    private readonly AppDbContext _db;

    public ProcessingJobRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProcessingJob>> GetFailedJobsAsync(CancellationToken ct)
    {
        return await _db.ProcessingJobs
            .Include(j => j.Order)
            .Where(j => j.Status == ProcessingJobStatus.Failed)
            .OrderByDescending(j => j.FailedAt ?? j.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<ProcessingJob>> GetByOrderIdAsync(int orderId, CancellationToken ct)
    {
        return await _db.ProcessingJobs
            .Where(j => j.OrderId == orderId)
            .OrderByDescending(j => j.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<ProcessingJob?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.ProcessingJobs
            .FirstOrDefaultAsync(j => j.ProcessingJobId == id, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
