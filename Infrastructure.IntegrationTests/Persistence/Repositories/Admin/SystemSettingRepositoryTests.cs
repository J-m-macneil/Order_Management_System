using Domain.Entities.SystemSettings;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Admin;

public class SystemSettingRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsSettingsInIdOrder()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new SystemSettingRepository(context);

        context.SystemSettings.AddRange(
            CreateSetting(9002, "IntegrationSecondSetting", "true", "boolean"),
            CreateSetting(9001, "IntegrationFirstSetting", "20", "integer"));
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetAllAsync(CancellationToken.None);

        // Assert
        var firstIndex = result.FindIndex(x => x.SystemSettingId == 9001);
        var secondIndex = result.FindIndex(x => x.SystemSettingId == 9002);

        firstIndex.Should().BeGreaterThanOrEqualTo(0);
        secondIndex.Should().BeGreaterThan(firstIndex);
    }

    [Fact]
    public async Task SaveChangesAsync_WithUpdatedSetting_PersistsNewValue()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new SystemSettingRepository(context);
        var setting = CreateSetting(9001, "IntegrationRetryLimit", "3", "integer");

        context.SystemSettings.Add(setting);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var existing = await repo.GetByIdAsync(setting.SystemSettingId, CancellationToken.None);
        existing!.SettingValue = "4";
        existing.UpdatedAt = new DateTime(2026, 6, 18);
        await repo.SaveChangesAsync(CancellationToken.None);

        // Assert
        var result = await context.SystemSettings.AsNoTracking()
            .SingleAsync(x => x.SystemSettingId == setting.SystemSettingId);

        result.SettingValue.Should().Be("4");
        result.UpdatedAt.Should().Be(new DateTime(2026, 6, 18));
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

    private static SystemSetting CreateSetting(int id, string key, string value, string dataType)
    {
        return new SystemSetting
        {
            SystemSettingId = id,
            SettingKey = key,
            SettingValue = value,
            DataType = dataType,
            Description = $"{key} description",
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
