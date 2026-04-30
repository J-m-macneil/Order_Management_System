using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class SystemSettingsService : ISystemSettingsService
{
    private readonly AppDbContext _context;

    public SystemSettingsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetStringAsync(string key)
    {
        var setting = await _context.SystemSettings
            .FirstAsync(x => x.SettingKey == key);

        return setting.SettingValue;
    }

    public async Task<int> GetIntAsync(string key)
    {
        var value = await GetStringAsync(key);
        return int.Parse(value);
    }

    public async Task<bool> GetBoolAsync(string key)
    {
        var value = await GetStringAsync(key);
        return bool.Parse(value);
    }
}