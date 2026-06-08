using Application.Common.Models;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, PagedResult<ProductListDto>>
{
    private readonly IProductRepository _repo;

    public GetProductsQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<ProductListDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var totalCount = await _repo.CountActiveAsync(
            request.SearchTerm,
            request.IsActive,
            request.IsRestricted,
            request.IsHazardous,
            request.ProductCategoryId,
            request.HazardClassId,
            ct);

        var products = await _repo.GetPagedAsync(
            request.Skip,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            request.IsRestricted,
            request.IsHazardous,
            request.ProductCategoryId,
            request.HazardClassId,
            ct);

        var items = products.Select(x => new ProductListDto
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

        return new PagedResult<ProductListDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
