using Domain.Entities;

namespace Domain.Repositories;

public interface ISafetyDataSheetRepository
{
    Task<List<SafetyDataSheet>> GetByProductIdAsync(int productId, CancellationToken ct);

    Task<SafetyDataSheet?> GetByIdAsync(int productId, int safetyDataSheetId, CancellationToken ct);

    Task AddAsync(SafetyDataSheet entity, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}