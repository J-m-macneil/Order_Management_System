using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetSafetyDataSheets;

public class GetSafetyDataSheetsQuery : IRequest<List<SafetyDataSheetDto>>
{
    public int ProductId { get; set; }
}