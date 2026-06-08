using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Products
            .Include(x => x.ProductCategory)
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.HazardClass)
            .Where(x => x.IsActive && x.DeletedAt == null)
            .ToListAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Products
            .Include(x => x.ProductCategory)
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.HazardClass)
            .FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null, ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<int> CountActiveAsync(
        string? searchTerm,
        bool? isActive,
        bool? isRestricted,
        bool? isHazardous,
        int? productCategoryId,
        int? hazardClassId,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.Products
                    .AsNoTracking()
                    .Include(x => x.ProductCategory)
                    .Include(x => x.UnitOfMeasure)
                    .Include(x => x.HazardClass),
                searchTerm,
                isActive,
                isRestricted,
                isHazardous,
                productCategoryId,
                hazardClassId)
            .CountAsync(ct);
    }

    public async Task<(int TotalProducts, int ActiveProducts, int RestrictedProducts, int HazardousProducts)> GetSummaryAsync(
        CancellationToken ct)
    {
        var products = _db.Products
            .AsNoTracking()
            .Include(x => x.HazardClass)
            .Where(x => x.DeletedAt == null);

        var totalProducts = await products.CountAsync(ct);
        var activeProducts = await products.CountAsync(x => x.IsActive, ct);
        var restrictedProducts = await products.CountAsync(x => x.IsRestricted, ct);
        var hazardousProducts = await products.CountAsync(x => x.HazardClass.Name != "Non-Hazardous", ct);

        return (totalProducts, activeProducts, restrictedProducts, hazardousProducts);
    }

    public async Task<List<Product>> GetPagedAsync(
        int skip,
        int take,
        string? searchTerm,
        bool? isActive,
        bool? isRestricted,
        bool? isHazardous,
        int? productCategoryId,
        int? hazardClassId,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.Products
                    .AsNoTracking()
                    .Include(x => x.ProductCategory)
                    .Include(x => x.UnitOfMeasure)
                    .Include(x => x.HazardClass),
                searchTerm,
                isActive,
                isRestricted,
                isHazardous,
                productCategoryId,
                hazardClassId)
            .OrderBy(x => x.ProductName)
            .ThenBy(x => x.ProductId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    private static IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        string? searchTerm,
        bool? isActive,
        bool? isRestricted,
        bool? isHazardous,
        int? productCategoryId,
        int? hazardClassId)
    {
        query = query.Where(x => x.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(x =>
                x.SKU.Contains(term) ||
                x.ProductName.Contains(term) ||
                (x.PackSize != null && x.PackSize.Contains(term)) ||
                x.ProductCategory.Name.Contains(term) ||
                x.UnitOfMeasure.Name.Contains(term) ||
                x.HazardClass.Name.Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (isRestricted.HasValue)
        {
            query = query.Where(x => x.IsRestricted == isRestricted.Value);
        }

        if (isHazardous.HasValue)
        {
            query = isHazardous.Value
                ? query.Where(x => x.HazardClass.Name != "Non-Hazardous")
                : query.Where(x => x.HazardClass.Name == "Non-Hazardous");
        }

        if (productCategoryId.HasValue)
        {
            query = query.Where(x => x.ProductCategoryId == productCategoryId.Value);
        }

        if (hazardClassId.HasValue)
        {
            query = query.Where(x => x.HazardClassId == hazardClassId.Value);
        }

        return query;
    }
}
