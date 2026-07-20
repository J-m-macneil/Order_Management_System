using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetSafetyDataSheetFile;

public class GetSafetyDataSheetFileQuery : IRequest<SafetyDataSheetFileDto>
{
    public int ProductId { get; set; }
    public int SafetyDataSheetId { get; set; }
}
