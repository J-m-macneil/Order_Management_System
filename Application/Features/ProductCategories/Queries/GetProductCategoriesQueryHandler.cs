using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.ProductCategories.Queries.GetProductCategories;

public class GetProductCategoriesQueryHandler
    : IRequestHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>
{
    private readonly IProductCategoryRepository _repo;

    public GetProductCategoriesQueryHandler(IProductCategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductCategoryDto>> Handle(
        GetProductCategoriesQuery request,
        CancellationToken ct)
    {
        var categories = await _repo.GetAllAsync(ct);

        return categories.Select(x => new ProductCategoryDto
        {
            ProductCategoryId = x.ProductCategoryId,
            Name = x.Name
        }).ToList();
    }
}