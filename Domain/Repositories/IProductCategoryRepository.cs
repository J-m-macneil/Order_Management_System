using Domain.Entities;

namespace Domain.Repositories;

public interface IProductCategoryRepository
{
    Task<List<ProductCategory>> GetAllAsync(CancellationToken ct);
}