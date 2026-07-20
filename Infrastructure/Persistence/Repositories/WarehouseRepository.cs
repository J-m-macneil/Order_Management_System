using Domain.Entities.Organisation;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _db;

    public WarehouseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Warehouse>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Warehouses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WarehouseId == id && x.IsActive, ct);
    }
}