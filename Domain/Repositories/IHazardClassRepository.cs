using Domain.Entities;
using Domain.Entities.Products;

namespace Domain.Repositories;

public interface IHazardClassRepository
{
    Task<List<HazardClass>> GetAllAsync(CancellationToken ct);
}