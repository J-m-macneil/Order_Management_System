namespace Application.DTOs;

public class SafetyDataSheetDto
{
    public int SafetyDataSheetId { get; set; }
    public int ProductId { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public DateTime EffectiveDate { get; set; }
    public DateTime UploadedAt { get; set; }

    public int UploadedByUserId { get; set; }
}