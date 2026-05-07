using Application.Features.Products.Commands.DeleteSafetyDataSheet;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.DeleteSafetyDataSheet;

public class DeleteSafetyDataSheetCommandHandler
    : IRequestHandler<DeleteSafetyDataSheetCommand, Unit>
{
    private readonly ISafetyDataSheetRepository _repo;

    public DeleteSafetyDataSheetCommandHandler(ISafetyDataSheetRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(DeleteSafetyDataSheetCommand request, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(
            request.ProductId,
            request.SafetyDataSheetId,
            ct);

        if (item == null)
            throw new Exception("Safety data sheet not found");

        // soft delete
        item.IsActive = false;
        item.DeletedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}