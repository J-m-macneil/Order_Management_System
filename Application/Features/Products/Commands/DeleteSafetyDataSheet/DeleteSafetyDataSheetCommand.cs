using MediatR;

namespace Application.Features.Products.Commands.DeleteSafetyDataSheet;

public class DeleteSafetyDataSheetCommand : IRequest<Unit>
{
    public int ProductId { get; set; }
    public int SafetyDataSheetId { get; set; }
}