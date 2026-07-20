using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Products;

public class SafetyDataSheetRepositoryTests
{
    [Fact]
    public async Task GetByProductIdAsync_ReturnsOnlyActiveNonDeletedSafetyDataSheetsForProduct()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new SafetyDataSheetRepository(context);
        var product = CreateProduct("IT-SDS-PROD-001", "SDS Product");
        var otherProduct = CreateProduct("IT-SDS-PROD-002", "Other SDS Product");

        context.Products.AddRange(product, otherProduct);
        await context.SaveChangesAsync(CancellationToken.None);

        var active = CreateSafetyDataSheet(product.ProductId, "active-sds.pdf");
        var inactive = CreateSafetyDataSheet(product.ProductId, "inactive-sds.pdf");
        inactive.IsActive = false;
        var deleted = CreateSafetyDataSheet(product.ProductId, "deleted-sds.pdf");
        deleted.DeletedAt = DateTime.UtcNow;
        var otherProductSds = CreateSafetyDataSheet(otherProduct.ProductId, "other-product-sds.pdf");

        context.SafetyDataSheets.AddRange(active, inactive, deleted, otherProductSds);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByProductIdAsync(product.ProductId, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].SafetyDataSheetId.Should().Be(active.SafetyDataSheetId);
        result[0].FileName.Should().Be(active.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSafetyDataSheetBelongsToProduct_ReturnsSafetyDataSheet()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new SafetyDataSheetRepository(context);
        var product = CreateProduct("IT-SDS-PROD-010", "SDS Lookup Product");

        context.Products.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        var item = CreateSafetyDataSheet(product.ProductId, "lookup-sds.pdf");
        context.SafetyDataSheets.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByIdAsync(
            product.ProductId,
            item.SafetyDataSheetId,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.SafetyDataSheetId.Should().Be(item.SafetyDataSheetId);
        result.ProductId.Should().Be(product.ProductId);
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsSoftDelete()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new SafetyDataSheetRepository(context);
        var product = CreateProduct("IT-SDS-PROD-020", "SDS Delete Product");

        context.Products.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        var item = CreateSafetyDataSheet(product.ProductId, "delete-sds.pdf");
        context.SafetyDataSheets.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        item.IsActive = false;
        item.DeletedAt = DateTime.UtcNow;

        // Act
        await repo.SaveChangesAsync(CancellationToken.None);

        var result = await repo.GetByIdAsync(
            product.ProductId,
            item.SafetyDataSheetId,
            CancellationToken.None);

        // Assert
        result.Should().BeNull();
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

    private static Product CreateProduct(string sku, string productName)
    {
        return new Product
        {
            SKU = sku,
            ProductName = productName,
            Description = "Integration test product",
            ProductCategoryId = 1,
            UnitOfMeasureId = 1,
            PackSize = "25L",
            BasePrice = 29.50m,
            Currency = "GBP",
            HazardClassId = 2,
            UNNumber = "UN1090",
            StorageRequirement = "Store securely",
            RequiresSds = true,
            IsRestricted = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static SafetyDataSheet CreateSafetyDataSheet(int productId, string fileName)
    {
        return new SafetyDataSheet
        {
            ProductId = productId,
            FileName = fileName,
            FilePath = $"/sds/{fileName}",
            Version = "1.0",
            EffectiveDate = new DateTime(2026, 1, 1),
            UploadedAt = new DateTime(2026, 1, 2),
            UploadedByUserId = 1,
            IsActive = true
        };
    }
}
