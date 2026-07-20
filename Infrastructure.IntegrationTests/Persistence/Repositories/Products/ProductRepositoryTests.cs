using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Products;

public class ProductRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WithExistingProduct_ReturnsProductWithLookups()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new ProductRepository(context);
        var product = CreateProduct("IT-PROD-001", "Integration Product");

        context.Products.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByIdAsync(product.ProductId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ProductId.Should().Be(product.ProductId);
        result.ProductCategory.Should().NotBeNull();
        result.UnitOfMeasure.Should().NotBeNull();
        result.HazardClass.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPagedAsync_WithFilters_ReturnsMatchingProductsInProductNameOrder()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new ProductRepository(context);
        var searchTerm = $"Repository Filter {Guid.NewGuid():N}";

        var beta = CreateProduct("IT-PROD-010", $"{searchTerm} Beta", isRestricted: true);
        var alpha = CreateProduct("IT-PROD-011", $"{searchTerm} Alpha", isRestricted: true);
        var unrestricted = CreateProduct("IT-PROD-012", $"{searchTerm} Unrestricted", isRestricted: false);
        var deleted = CreateProduct("IT-PROD-013", $"{searchTerm} Deleted", isRestricted: true);
        deleted.DeletedAt = DateTime.UtcNow;

        context.Products.AddRange(beta, alpha, unrestricted, deleted);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var total = await repo.CountActiveAsync(
            searchTerm,
            isActive: true,
            isRestricted: true,
            isHazardous: true,
            productCategoryId: 1,
            hazardClassId: 2,
            CancellationToken.None);

        var result = await repo.GetPagedAsync(
            skip: 0,
            take: 10,
            searchTerm,
            isActive: true,
            isRestricted: true,
            isHazardous: true,
            productCategoryId: 1,
            hazardClassId: 2,
            CancellationToken.None);

        // Assert
        total.Should().Be(2);
        result.Should().HaveCount(2);
        result.Select(x => x.ProductId).Should().Equal(alpha.ProductId, beta.ProductId);
        result.Should().OnlyContain(x =>
            x.ProductName.Contains(searchTerm) &&
            x.IsActive &&
            x.IsRestricted &&
            x.HazardClassId == 2 &&
            x.ProductCategoryId == 1 &&
            x.DeletedAt == null);
    }

    [Fact]
    public async Task UpdateAsync_PersistsProductChanges()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new ProductRepository(context);
        var product = CreateProduct("IT-PROD-020", "Original Product");

        context.Products.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        product.ProductName = "Updated Product";
        product.BasePrice = 42.50m;
        product.IsRestricted = true;

        // Act
        await repo.UpdateAsync(product, CancellationToken.None);

        var result = await repo.GetByIdAsync(product.ProductId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ProductName.Should().Be("Updated Product");
        result.BasePrice.Should().Be(42.50m);
        result.IsRestricted.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductIsSoftDeleted_ReturnsNull()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new ProductRepository(context);
        var product = CreateProduct("IT-PROD-030", "Deleted Product");

        context.Products.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        product.IsActive = false;
        product.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByIdAsync(product.ProductId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSummaryAsync_ReturnsCountsExcludingDeletedProducts()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new ProductRepository(context);
        var before = await repo.GetSummaryAsync(CancellationToken.None);

        var activeRestrictedHazardous = CreateProduct("IT-PROD-040", "Summary Active Restricted", isRestricted: true);
        var inactiveNonHazardous = CreateProduct(
            "IT-PROD-041",
            "Summary Inactive Non Hazardous",
            isActive: false,
            hazardClassId: 1);
        var deleted = CreateProduct("IT-PROD-042", "Summary Deleted");
        deleted.DeletedAt = DateTime.UtcNow;

        context.Products.AddRange(activeRestrictedHazardous, inactiveNonHazardous, deleted);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetSummaryAsync(CancellationToken.None);

        // Assert
        result.TotalProducts.Should().Be(before.TotalProducts + 2);
        result.ActiveProducts.Should().Be(before.ActiveProducts + 1);
        result.RestrictedProducts.Should().Be(before.RestrictedProducts + 1);
        result.HazardousProducts.Should().Be(before.HazardousProducts + 1);
    }

    private static async Task<AppDbContext> CreateContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return context;
    }

    private static Product CreateProduct(
        string sku,
        string productName,
        bool isActive = true,
        bool isRestricted = false,
        int productCategoryId = 1,
        int unitOfMeasureId = 1,
        int hazardClassId = 2)
    {
        return new Product
        {
            SKU = sku,
            ProductName = productName,
            Description = "Integration test product",
            ProductCategoryId = productCategoryId,
            UnitOfMeasureId = unitOfMeasureId,
            PackSize = "25L",
            BasePrice = 29.50m,
            Currency = "GBP",
            HazardClassId = hazardClassId,
            UNNumber = hazardClassId == 1 ? null : "UN1090",
            StorageRequirement = "Store securely",
            RequiresSds = hazardClassId != 1,
            IsRestricted = isRestricted,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }
}
