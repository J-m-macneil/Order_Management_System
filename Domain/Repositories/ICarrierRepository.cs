using Domain.Entities.Organisation;

namespace Domain.Repositories;

public interface ICarrierRepository
{
    Task<List<Carrier>> GetActiveAsync(CancellationToken ct);
    Task<Carrier?> GetByIdAsync(int id, CancellationToken ct);
}