using Application.Features.Products.Queries.GetProductById;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsProductDto()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var handler = new GetProductByIdQueryHandler(repo);
        var product = CreateProduct();
        var query = new GetProductByIdQuery { Id = product.ProductId };

        repo.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ProductId.Should().Be(product.ProductId);
        result.SKU.Should().Be(product.SKU);
        result.ProductName.Should().Be(product.ProductName);
        result.Description.Should().Be(product.Description);
        result.ProductCategoryId.Should().Be(product.ProductCategoryId);
        result.UnitOfMeasureId.Should().Be(product.UnitOfMeasureId);
        result.PackSize.Should().Be(product.PackSize);
        result.BasePrice.Should().Be(product.BasePrice);
        result.Currency.Should().Be(product.Currency);
        result.HazardClassId.Should().Be(product.HazardClassId);
        result.UNNumber.Should().Be(product.UNNumber);
        result.StorageRequirement.Should().Be(product.StorageRequirement);
        result.RequiresSds.Should().Be(product.RequiresSds);
        result.IsRestricted.Should().Be(product.IsRestricted);
        result.IsActive.Should().Be(product.IsActive);

        await repo.Received(1)
            .GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ThrowsException()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var handler = new GetProductByIdQueryHandler(repo);
        var query = new GetProductByIdQuery { Id = 123 };

        repo.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // Act
        var act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Product not found");

        await repo.Received(1)
            .GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
    }

    private static Product CreateProduct()
    {
        return new Product
        {
            ProductId = 123,
            SKU = "CHEM-001",
            ProductName = "Acetone",
            Description = "Industrial solvent",
            ProductCategoryId = 2,
            UnitOfMeasureId = 3,
            PackSize = "25L",
            BasePrice = 29.50m,
            Currency = "GBP",
            HazardClassId = 4,
            UNNumber = "UN1090",
            StorageRequirement = "Store in a flammable cabinet",
            RequiresSds = true,
            IsRestricted = true,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
