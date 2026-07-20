using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetSafetyDataSheets;

public class GetSafetyDataSheetsQueryHandler
    : IRequestHandler<GetSafetyDataSheetsQuery, List<SafetyDataSheetDto>>
{
    private readonly ISafetyDataSheetRepository _repo;

    public GetSafetyDataSheetsQueryHandler(ISafetyDataSheetRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SafetyDataSheetDto>> Handle(GetSafetyDataSheetsQuery request, CancellationToken ct)
    {
        var items = await _repo.GetByProductIdAsync(request.ProductId, ct);

        return items.Select(x => new SafetyDataSheetDto
        {
            SafetyDataSheetId = x.SafetyDataSheetId,
            ProductId = x.ProductId,
            FileName = x.FileName,
            FilePath = x.FilePath,
            Version = x.Version,
            EffectiveDate = x.EffectiveDate,
            UploadedAt = x.UploadedAt,
            UploadedByUserId = x.UploadedByUserId,
            UploadedByUserName = x.UploadedByUser.FullName,
            IsActive = x.IsActive
        }).ToList();
    }
}
