using Domain.Entities;

namespace Domain.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<int> CountActiveAsync(CancellationToken ct);
    Task<List<Product>> GetPagedAsync(
        int skip,
        int take,
        CancellationToken ct);
}
