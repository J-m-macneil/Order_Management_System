using Application.Features.Products.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.CreateSafetyDataSheet;

public class CreateSafetyDataSheetCommandHandler
    : IRequestHandler<CreateSafetyDataSheetCommand, SafetyDataSheetDto>
{
    private readonly ISafetyDataSheetRepository _repo;
    private readonly IAuditService _audit;

    public CreateSafetyDataSheetCommandHandler(
        ISafetyDataSheetRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<SafetyDataSheetDto> Handle(CreateSafetyDataSheetCommand request, CancellationToken ct)
    {
        var item = new SafetyDataSheet
        {
            ProductId = request.ProductId,
            FileName = request.FileName,
            FilePath = request.FilePath,
            Version = request.Version,
            EffectiveDate = request.EffectiveDate,
            UploadedAt = request.UploadedAt,
            UploadedByUserId = request.UploadedByUserId,
            IsActive = true
        };

        await _repo.AddAsync(item, ct);

        await _audit.LogAsync(
            "SafetyDataSheet",
            item.SafetyDataSheetId,
            "Added",
            null,
            CreateSnapshot(item),
            $"Safety data sheet added for product #{item.ProductId}: {item.FileName}.",
            ct);

        return new SafetyDataSheetDto
        {
            SafetyDataSheetId = item.SafetyDataSheetId,
            ProductId = item.ProductId,
            FileName = item.FileName,
            FilePath = item.FilePath,
            Version = item.Version,
            EffectiveDate = item.EffectiveDate,
            UploadedAt = item.UploadedAt,
            UploadedByUserId = item.UploadedByUserId
        };
    }

    private static object CreateSnapshot(SafetyDataSheet item)
    {
        return new
        {
            item.SafetyDataSheetId,
            item.ProductId,
            item.FileName,
            item.FilePath,
            item.Version,
            item.EffectiveDate,
            item.UploadedAt,
            item.UploadedByUserId,
            item.IsActive,
            item.DeletedAt
        };
    }
}
