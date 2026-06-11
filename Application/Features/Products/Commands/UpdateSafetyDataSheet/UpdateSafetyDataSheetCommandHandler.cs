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
    private readonly IAuditChangeFormatter _changeFormatter;

    public UpdateSafetyDataSheetCommandHandler(
        ISafetyDataSheetRepository repo,
        IAuditService audit,
        IAuditChangeFormatter changeFormatter)
    {
        _repo = repo;
        _audit = audit;
        _changeFormatter = changeFormatter;
    }

    public async Task<Unit> Handle(UpdateSafetyDataSheetCommand request, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(
            request.ProductId,
            request.SafetyDataSheetId,
            ct);

        if (item == null)
            throw new Exception("Safety data sheet not found");

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
        var changes = _changeFormatter.GetChanges(oldValues, newValues);

        await _audit.LogAsync(
            "SafetyDataSheet",
            item.SafetyDataSheetId,
            "Updated",
            oldValues,
            newValues,
            _changeFormatter.CreateUpdateNote(
                "Safety data sheet",
                $"{item.FileName} for product #{item.ProductId}",
                changes),
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
