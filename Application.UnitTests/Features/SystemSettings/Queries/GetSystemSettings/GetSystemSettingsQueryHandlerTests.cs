using Application.Features.SystemSettings.Queries.GetSystemSettings;
using Domain.Entities.SystemSettings;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.SystemSettings.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSettingsOrderedByRepositoryResult()
    {
        // Arrange
        var repo = Substitute.For<ISystemSettingRepository>();
        var handler = new GetSystemSettingsQueryHandler(repo);
        var settings = new List<SystemSetting>
        {
            new()
            {
                SystemSettingId = 1,
                SettingKey = "DefaultTaxRate",
                SettingValue = "20",
                DataType = "integer",
                Description = "Default VAT rate",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new()
            {
                SystemSettingId = 2,
                SettingKey = "EnablePriorityOrders",
                SettingValue = "true",
                DataType = "boolean",
                Description = "Enable priority orders",
                CreatedAt = new DateTime(2026, 1, 1)
            }
        };

        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(settings);

        // Act
        var result = await handler.Handle(new GetSystemSettingsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].SettingKey.Should().Be("DefaultTaxRate");
        result[0].SettingValue.Should().Be("20");
        result[1].SettingKey.Should().Be("EnablePriorityOrders");
        result[1].DataType.Should().Be("boolean");
    }
}
