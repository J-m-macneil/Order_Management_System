using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.UpdateSafetyDataSheet;

public class UpdateSafetyDataSheetCommandHandler
    : IRequestHandler<UpdateSafetyDataSheetCommand, Unit>
{
    private readonly ISafetyDataSheetRepository _repo;
    private readonly IAuditService _audit;

    public UpdateSafetyDataSheetCommandHandler(
        ISafetyDataSheetRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<Unit> Handle(UpdateSafetyDataSheetCommand request, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(
            request.ProductId,
            request.SafetyDataSheetId,
            ct);

        if (item == null)
            throw new NotFoundException("Safety data sheet was not found.");

        var oldValues = CreateSnapshot(item);

        item.FileName = request.FileName;
        item.FilePath = request.FilePath;
        item.Version = request.Version;
        item.EffectiveDate = request.EffectiveDate;
        item.UploadedAt = request.UploadedAt;
        item.UploadedByUserId = request.UploadedByUserId;
        item.IsActive = request.IsActive;

        await _repo.SaveChangesAsync(ct);

        var newValues = CreateSnapshot(item);

        await _audit.LogAsync(
            "SafetyDataSheet",
            item.SafetyDataSheetId,
            "Updated",
            oldValues,
            newValues,
            $"Safety data sheet updated: {item.FileName} for product #{item.ProductId}.",
            ct);

        return Unit.Value;
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
