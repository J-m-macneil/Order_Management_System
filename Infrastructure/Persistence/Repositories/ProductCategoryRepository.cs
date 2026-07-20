using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _db;

    public ProductCategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductCategory>> GetAllAsync(CancellationToken ct)
    {
        return await _db.ProductCategories
            .AsNoTracking()
            .ToListAsync(ct);
    }
}