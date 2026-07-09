using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductRequest, Unit>
{
    private readonly IProductRepository _repo;
    private readonly IAuditService _audit;

    public UpdateProductCommandHandler(
        IProductRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<Unit> Handle(UpdateProductRequest request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.ProductId, ct);

        if (product == null)
            throw new Exception("Product not found");

        var oldValues = CreateSnapshot(product);
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

        var newValues = CreateSnapshot(product);

        await _audit.LogAsync(
            "Product",
            product.ProductId,
            "Updated",
            oldValues,
            newValues,
            $"Product updated: {product.ProductName}.",
            ct);

        return Unit.Value;
    }

    private static object CreateSnapshot(Product product)
    {
        return new
        {
            product.SKU,
            product.ProductName,
            product.Description,
            product.ProductCategoryId,
            product.UnitOfMeasureId,
            product.PackSize,
            product.BasePrice,
            product.Currency,
            product.HazardClassId,
            product.UNNumber,
            product.StorageRequirement,
            product.RequiresSds,
            product.IsRestricted,
            product.IsActive
        };
    }

}
