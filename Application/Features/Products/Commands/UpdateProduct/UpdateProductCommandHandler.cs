using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductRequest, Unit>
{
    private readonly IProductRepository _repo;

    public UpdateProductCommandHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(UpdateProductRequest request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.ProductId, ct);

        if (product == null)
            throw new Exception("Product not found");

        var dto = request.Data;

        product.SKU = dto.SKU;
        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.ProductCategoryId = dto.ProductCategoryId;
        product.UnitOfMeasureId = dto.UnitOfMeasureId;
        product.PackSize = dto.PackSize;
        product.BasePrice = dto.BasePrice;
        product.Currency = dto.Currency;
        product.HazardClassId = dto.HazardClassId;
        product.UNNumber = dto.UNNumber;
        product.StorageRequirement = dto.StorageRequirement;
        product.RequiresSds = dto.RequiresSds;
        product.IsRestricted = dto.IsRestricted;
        product.IsActive = dto.IsActive;

        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}