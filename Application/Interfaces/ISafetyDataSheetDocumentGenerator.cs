using Application.Features.Products.DTOs;

namespace Application.Interfaces;

public interface ISafetyDataSheetDocumentGenerator
{
    Task<SafetyDataSheetDto> GenerateAsync(
        int productId,
        int generatedByUserId,
        CancellationToken cancellationToken = default);
}
