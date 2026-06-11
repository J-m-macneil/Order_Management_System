using Application.Features.Products.Queries.GetProducts;
using Domain.Entities;
using Domain.Entities.Products;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithPagedProducts_ReturnsPagedResult()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var handler = new GetProductsQueryHandler(repo);
        var query = new GetProductsQuery
        {
            PageNumber = 2,
            PageSize = 10,
            SearchTerm = "acetone",
            IsActive = true,
            IsRestricted = true,
            IsHazardous = true,
            ProductCategoryId = 2,
            HazardClassId = 4
        };

        var products = new List<Product>
        {
            CreateProduct()
        };

        repo.CountActiveAsync(
            query.SearchTerm,
            query.IsActive,
            query.IsRestricted,
            query.IsHazardous,
            query.ProductCategoryId,
            query.HazardClassId,
            Arg.Any<CancellationToken>())
            .Returns(25);

        repo.GetPagedAsync(
            query.Skip,
            query.PageSize,
            query.SearchTerm,
            query.IsActive,
            query.IsRestricted,
            query.IsHazardous,
            query.ProductCategoryId,
            query.HazardClassId,
            Arg.Any<CancellationToken>())
            .Returns(products);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(25);
        result.Items.Should().ContainSingle();

        var item = result.Items.Single();
        item.ProductId.Should().Be(products[0].ProductId);
        item.SKU.Should().Be(products[0].SKU);
        item.ProductName.Should().Be(products[0].ProductName);
        item.ProductCategoryName.Should().Be(products[0].ProductCategory.Name);
        item.UnitOfMeasureName.Should().Be(products[0].UnitOfMeasure.Name);
        item.HazardClassName.Should().Be(products[0].HazardClass.Name);
        item.PackSize.Should().Be(products[0].PackSize);
        item.BasePrice.Should().Be(products[0].BasePrice);
        item.Currency.Should().Be(products[0].Currency);
        item.IsRestricted.Should().Be(products[0].IsRestricted);
        item.IsActive.Should().Be(products[0].IsActive);

        await repo.Received(1).CountActiveAsync(
            query.SearchTerm,
            query.IsActive,
            query.IsRestricted,
            query.IsHazardous,
            query.ProductCategoryId,
            query.HazardClassId,
            Arg.Any<CancellationToken>());

        await repo.Received(1).GetPagedAsync(
            query.Skip,
            query.PageSize,
            query.SearchTerm,
            query.IsActive,
            query.IsRestricted,
            query.IsHazardous,
            query.ProductCategoryId,
            query.HazardClassId,
            Arg.Any<CancellationToken>());
    }

    private static Product CreateProduct()
    {
        return new Product
        {
            ProductId = 123,
            SKU = "CHEM-001",
            ProductName = "Acetone",
            ProductCategoryId = 2,
            ProductCategory = new ProductCategory { ProductCategoryId = 2, Name = "Solvents" },
            UnitOfMeasureId = 3,
            UnitOfMeasure = new UnitOfMeasure { UnitOfMeasureId = 3, Name = "Litre", Code = "L" },
            HazardClassId = 4,
            HazardClass = new HazardClass { HazardClassId = 4, Name = "Flammable" },
            PackSize = "25L",
            BasePrice = 29.50m,
            Currency = "GBP",
            IsRestricted = true,
            IsActive = true
        };
    }
}
