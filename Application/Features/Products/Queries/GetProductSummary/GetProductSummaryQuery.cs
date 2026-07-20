using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetProductSummary;

public class GetProductSummaryQuery : IRequest<ProductSummaryDto>
{
}
