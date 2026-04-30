public interface ISystemSettingsService
{
    Task<int> GetIntAsync(string key);
    Task<bool> GetBoolAsync(string key);
    Task<string> GetStringAsync(string key);
}