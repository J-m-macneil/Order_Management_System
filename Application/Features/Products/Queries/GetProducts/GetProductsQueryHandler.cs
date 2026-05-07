using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, List<ProductListDto>>
{
    private readonly IProductRepository _repo;

    public GetProductsQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductListDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var products = await _repo.GetAllAsync(ct);

        return products.Select(x => new ProductListDto
        {
            ProductId = x.ProductId,
            SKU = x.SKU,
            ProductName = x.ProductName,
            ProductCategoryName = x.ProductCategory.Name,
            UnitOfMeasureName = x.UnitOfMeasure.Name,
            HazardClassName = x.HazardClass.Name,
            PackSize = x.PackSize,
            BasePrice = x.BasePrice,
            Currency = x.Currency,
            IsRestricted = x.IsRestricted,
            IsActive = x.IsActive
        }).ToList();
    }
}