using Domain.Entities.SystemSettings;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SystemSettingRepository : ISystemSettingRepository
{
    private readonly AppDbContext _db;

    public SystemSettingRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<SystemSetting>> GetAllAsync(CancellationToken ct)
    {
        return _db.SystemSettings
            .AsNoTracking()
            .OrderBy(x => x.SystemSettingId)
            .ToListAsync(ct);
    }

    public Task<SystemSetting?> GetByIdAsync(int systemSettingId, CancellationToken ct)
    {
        return _db.SystemSettings
            .FirstOrDefaultAsync(x => x.SystemSettingId == systemSettingId, ct);
    }

    public Task<SystemSetting?> GetByKeyAsync(string settingKey, CancellationToken ct)
    {
        return _db.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SettingKey == settingKey, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
}
