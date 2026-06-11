using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Commands.CreateSafetyDataSheet;

public class CreateSafetyDataSheetCommand : IRequest<SafetyDataSheetDto>
{
    public int ProductId { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public DateTime EffectiveDate { get; set; }
    public DateTime UploadedAt { get; set; }

    public int UploadedByUserId { get; set; }
}
