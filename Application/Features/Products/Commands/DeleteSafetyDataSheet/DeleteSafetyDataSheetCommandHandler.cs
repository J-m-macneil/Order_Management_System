using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.DeleteSafetyDataSheet;

public class DeleteSafetyDataSheetCommandHandler
    : IRequestHandler<DeleteSafetyDataSheetCommand, Unit>
{
    private readonly ISafetyDataSheetRepository _repo;
    private readonly IAuditService _audit;

    public DeleteSafetyDataSheetCommandHandler(
        ISafetyDataSheetRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<Unit> Handle(DeleteSafetyDataSheetCommand request, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(
            request.ProductId,
            request.SafetyDataSheetId,
            ct);

        if (item == null)
            throw new Exception("Safety data sheet not found");

        var oldValues = CreateSnapshot(item);

        item.IsActive = false;
        item.DeletedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "SafetyDataSheet",
            item.SafetyDataSheetId,
            "Deleted",
            oldValues,
            CreateSnapshot(item),
            $"Safety data sheet deleted for product #{item.ProductId}: {item.FileName}.",
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
