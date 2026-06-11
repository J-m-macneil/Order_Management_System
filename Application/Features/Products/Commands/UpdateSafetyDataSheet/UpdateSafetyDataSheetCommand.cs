using MediatR;

namespace Application.Features.Products.Commands.UpdateSafetyDataSheet;

public class UpdateSafetyDataSheetCommand : IRequest<Unit>
{
    public int ProductId { get; set; }
    public int SafetyDataSheetId { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public DateTime EffectiveDate { get; set; }
    public DateTime UploadedAt { get; set; }

    public int UploadedByUserId { get; set; }
    public bool IsActive { get; set; }
}
