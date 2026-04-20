namespace Domain.Entities;

public class SafetyDataSheet
{
    public int SafetyDataSheetId { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public DateTime EffectiveDate { get; set; }
    public DateTime UploadedAt { get; set; }

    public int UploadedByUserId { get; set; }
    public User UploadedByUser { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }
}