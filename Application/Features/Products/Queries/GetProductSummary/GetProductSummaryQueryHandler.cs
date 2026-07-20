using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetProductSummary;

public class GetProductSummaryQueryHandler : IRequestHandler<GetProductSummaryQuery, ProductSummaryDto>
{
    private readonly IProductRepository _repo;

    public GetProductSummaryQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductSummaryDto> Handle(GetProductSummaryQuery request, CancellationToken ct)
    {
        var summary = await _repo.GetSummaryAsync(ct);

        return new ProductSummaryDto
        {
            TotalProducts = summary.TotalProducts,
            ActiveProducts = summary.ActiveProducts,
            RestrictedProducts = summary.RestrictedProducts,
            HazardousProducts = summary.HazardousProducts
        };
    }
}
