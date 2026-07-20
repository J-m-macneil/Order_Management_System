using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Commands.GenerateSafetyDataSheet;

public class GenerateSafetyDataSheetCommand : IRequest<SafetyDataSheetDto>
{
    public int ProductId { get; set; }
}
