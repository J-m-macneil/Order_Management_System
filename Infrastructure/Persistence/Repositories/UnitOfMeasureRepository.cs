using Domain.Entities;
using Domain.Entities.Products;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly AppDbContext _db;

    public UnitOfMeasureRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UnitOfMeasure>> GetAllAsync(CancellationToken ct)
    {
        return await _db.UnitsOfMeasure
            .AsNoTracking()
            .ToListAsync(ct);
    }
}