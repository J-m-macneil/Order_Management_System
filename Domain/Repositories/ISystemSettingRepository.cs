using Domain.Entities.SystemSettings;

namespace Domain.Repositories;

public interface ISystemSettingRepository
{
    Task<List<SystemSetting>> GetAllAsync(CancellationToken ct);
    Task<SystemSetting?> GetByIdAsync(int systemSettingId, CancellationToken ct);
    Task<SystemSetting?> GetByKeyAsync(string settingKey, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
