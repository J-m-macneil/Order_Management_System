using Domain.Entities;
using Domain.Entities.Products;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class HazardClassRepository : IHazardClassRepository
{
    private readonly AppDbContext _db;

    public HazardClassRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<HazardClass>> GetAllAsync(CancellationToken ct)
    {
        return await _db.HazardClasses
            .AsNoTracking()
            .ToListAsync(ct);
    }
}