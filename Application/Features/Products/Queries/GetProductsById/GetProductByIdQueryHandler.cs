using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repo;

    public GetProductByIdQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var x = await _repo.GetByIdAsync(request.Id, ct);

        if (x == null)
            throw new Exception("Product not found");

        return new ProductDto
        {
            ProductId = x.ProductId,
            SKU = x.SKU,
            ProductName = x.ProductName,
            Description = x.Description,
            ProductCategoryId = x.ProductCategoryId,
            UnitOfMeasureId = x.UnitOfMeasureId,
            PackSize = x.PackSize,
            BasePrice = x.BasePrice,
            Currency = x.Currency,
            HazardClassId = x.HazardClassId,
            UNNumber = x.UNNumber,
            StorageRequirement = x.StorageRequirement,
            RequiresSds = x.RequiresSds,
            IsRestricted = x.IsRestricted,
            IsActive = x.IsActive
        };
    }
}