using Domain.Entities;
using Domain.Entities.Products;

namespace Domain.Repositories;

public interface IUnitOfMeasureRepository
{
    Task<List<UnitOfMeasure>> GetAllAsync(CancellationToken ct);
}