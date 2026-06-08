using Domain.Entities;

namespace Domain.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<int> CountActiveAsync(
        string? searchTerm,
        bool? isActive,
        bool? isRestricted,
        bool? isHazardous,
        int? productCategoryId,
        int? hazardClassId,
        CancellationToken ct);

    Task<(int TotalProducts, int ActiveProducts, int RestrictedProducts, int HazardousProducts)> GetSummaryAsync(
        CancellationToken ct);

    Task<List<Product>> GetPagedAsync(
        int skip,
        int take,
        string? searchTerm,
        bool? isActive,
        bool? isRestricted,
        bool? isHazardous,
        int? productCategoryId,
        int? hazardClassId,
        CancellationToken ct);
}
