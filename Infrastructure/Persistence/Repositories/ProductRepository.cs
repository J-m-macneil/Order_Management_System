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

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
