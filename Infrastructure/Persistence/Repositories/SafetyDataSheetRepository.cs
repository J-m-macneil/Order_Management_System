using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SafetyDataSheetRepository : ISafetyDataSheetRepository
{
    private readonly AppDbContext _db;

    public SafetyDataSheetRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SafetyDataSheet>> GetByProductIdAsync(int productId, CancellationToken ct)
    {
        return await _db.SafetyDataSheets
            .Where(x => x.ProductId == productId && x.IsActive && x.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<SafetyDataSheet?> GetByIdAsync(int id, int productId, CancellationToken ct)
    {
        return await _db.SafetyDataSheets
            .FirstOrDefaultAsync(x =>
                x.SafetyDataSheetId == id &&
                x.ProductId == productId &&
                x.DeletedAt == null, ct);
    }

    public async Task AddAsync(SafetyDataSheet entity, CancellationToken ct)
    {
        _db.SafetyDataSheets.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}