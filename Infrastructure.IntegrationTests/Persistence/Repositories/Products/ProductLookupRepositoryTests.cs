using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Products;

public class ProductLookupRepositoryTests
{
    [Fact]
    public async Task ProductCategoryRepository_GetAllAsync_ReturnsSeededCategories()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new ProductCategoryRepository(context);

        // Act
        var result = await repo.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().Contain(x => x.Name == "Solvents");
        result.Should().Contain(x => x.Name == "Cleaning Chemicals");
    }

    [Fact]
    public async Task UnitOfMeasureRepository_GetAllAsync_ReturnsSeededUnitsOfMeasure()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new UnitOfMeasureRepository(context);

        // Act
        var result = await repo.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().Contain(x => x.Code == "L" && x.Name == "Litre");
        result.Should().Contain(x => x.Code == "KG" && x.Name == "Kilogram");
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
}
