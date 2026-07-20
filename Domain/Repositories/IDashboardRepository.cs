using Domain.Models;

namespace Domain.Repositories;

public interface IDashboardRepository
{
    Task<DashboardMetrics> GetMetricsAsync(CancellationToken ct);
}