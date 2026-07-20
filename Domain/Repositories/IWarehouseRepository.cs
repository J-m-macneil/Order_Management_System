using Domain.Entities.Organisation;

namespace Domain.Repositories;

public interface IWarehouseRepository
{
    Task<List<Warehouse>> GetAllAsync(CancellationToken ct);
    Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct);
}