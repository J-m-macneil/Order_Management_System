using Application.Common.Exceptions;
using Application.Features.SystemSettings.Commands.UpdateSystemSetting;
using Application.Interfaces;
using Domain.Entities.SystemSettings;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.SystemSettings.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidIntegerValue_UpdatesSettingAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ISystemSettingRepository>();
        var audit = Substitute.For<IAuditService>();
        var handler = new UpdateSystemSettingCommandHandler(repo, audit);
        var setting = new SystemSetting
        {
            SystemSettingId = 1,
            SettingKey = "DefaultTaxRate",
            SettingValue = "20",
            DataType = "integer",
            Description = "Default VAT rate",
            CreatedAt = new DateTime(2026, 1, 1)
        };

        repo.GetByIdAsync(setting.SystemSettingId, Arg.Any<CancellationToken>())
            .Returns(setting);

        var request = new UpdateSystemSettingRequest
        {
            SystemSettingId = setting.SystemSettingId,
            Data = new UpdateSystemSettingCommand { SettingValue = "21" }
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        setting.SettingValue.Should().Be("21");
        setting.UpdatedAt.Should().NotBeNull();

        await repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "SystemSetting"),
            Arg.Is<int>(value => value == setting.SystemSettingId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains(setting.SettingKey)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidBooleanValue_ThrowsAndDoesNotSave()
    {
        // Arrange
        var repo = Substitute.For<ISystemSettingRepository>();
        var audit = Substitute.For<IAuditService>();
        var handler = new UpdateSystemSettingCommandHandler(repo, audit);
        var setting = new SystemSetting
        {
            SystemSettingId = 2,
            SettingKey = "EnablePriorityOrders",
            SettingValue = "true",
            DataType = "boolean",
            CreatedAt = new DateTime(2026, 1, 1)
        };

        repo.GetByIdAsync(setting.SystemSettingId, Arg.Any<CancellationToken>())
            .Returns(setting);

        var request = new UpdateSystemSettingRequest
        {
            SystemSettingId = setting.SystemSettingId,
            Data = new UpdateSystemSettingCommand { SettingValue = "sometimes" }
        };

        // Act
        var act = () => handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("EnablePriorityOrders must be true or false.");

        setting.SettingValue.Should().Be("true");
        await repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }
}
