public class SystemSetting
{
    public int SystemSettingId { get; set; }

    public string SettingKey { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
    public string DataType { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}