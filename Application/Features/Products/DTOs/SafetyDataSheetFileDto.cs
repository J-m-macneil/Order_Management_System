namespace Application.Features.Products.DTOs;

public class SafetyDataSheetFileDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public byte[] Content { get; set; } = [];
}
