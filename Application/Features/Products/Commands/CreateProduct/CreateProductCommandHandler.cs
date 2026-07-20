using Application.Features.Products.DTOs;
using Application.Common.Exceptions;
using Application.Interfaces;
using Application.Common.Validation;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly IAuditService _audit;

    public CreateProductCommandHandler(
        IProductRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        ValidateRequest(request);

        var sku = request.SKU.Trim();

        if (await _repo.SkuExistsAsync(sku, null, ct))
        {
            throw new ConflictException("A product with this SKU already exists.");
        }

        var product = new Product
        {
            SKU = sku,
            ProductName = request.ProductName,
            Description = request.Description,
            ProductCategoryId = request.ProductCategoryId,
            UnitOfMeasureId = request.UnitOfMeasureId,
            PackSize = request.PackSize,
            BasePrice = request.BasePrice,
            Currency = request.Currency,
            HazardClassId = request.HazardClassId,
            UNNumber = request.UNNumber,
            StorageRequirement = request.StorageRequirement,
            RequiresSds = request.RequiresSds,
            IsRestricted = request.IsRestricted,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(product, ct);

        await _audit.LogAsync(
            "Product",
            product.ProductId,
            "Created",
            null,
            CreateSnapshot(product),
            $"Product created: {product.ProductName}.",
            ct);

        return new ProductDto
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            ProductName = product.ProductName,
            Description = product.Description,
            ProductCategoryId = product.ProductCategoryId,
            UnitOfMeasureId = product.UnitOfMeasureId,
            PackSize = product.PackSize,
            BasePrice = product.BasePrice,
            Currency = product.Currency,
            HazardClassId = product.HazardClassId,
            UNNumber = product.UNNumber,
            StorageRequirement = product.StorageRequirement,
            RequiresSds = product.RequiresSds,
            IsRestricted = product.IsRestricted,
            IsActive = product.IsActive
        };
    }

    private static void ValidateRequest(CreateProductCommand request)
    {
        CommandValidation.RequiredText(request.SKU, "SKU", 40);
        CommandValidation.RequiredText(request.ProductName, "Product name", 160);
        CommandValidation.OptionalText(request.Description, "Description", 255);
        CommandValidation.PositiveId(request.ProductCategoryId, "Product category");
        CommandValidation.PositiveId(request.UnitOfMeasureId, "Unit of measure");
        CommandValidation.RequiredText(request.PackSize, "Pack size", 40);
        CommandValidation.NonNegative(request.BasePrice, "Base price");
        CommandValidation.Currency(request.Currency);
        CommandValidation.PositiveId(request.HazardClassId, "Hazard class");
        CommandValidation.OptionalText(request.UNNumber, "UN number", 20);
        CommandValidation.OptionalText(request.StorageRequirement, "Storage requirement", 120);
    }

    private static object CreateSnapshot(Product product)
    {
        return new
        {
            product.ProductId,
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
