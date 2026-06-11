using Application.Features.Products.Commands.CreateSafetyDataSheet;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Commands.CreateSafetyDataSheet;

public class CreateSafetyDataSheetCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsSafetyDataSheetAndReturnsDto()
    {
        // Arrange
        SafetyDataSheet? savedSafetyDataSheet = null;

        var repo = Substitute.For<ISafetyDataSheetRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new CreateSafetyDataSheetCommandHandler(repo, audit);
        var command = CreateValidCommand();

        repo.AddAsync(Arg.Do<SafetyDataSheet>(item =>
        {
            item.SafetyDataSheetId = 456;
            savedSafetyDataSheet = item;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        savedSafetyDataSheet.Should().NotBeNull();

        savedSafetyDataSheet!.SafetyDataSheetId.Should().Be(456);
        savedSafetyDataSheet.ProductId.Should().Be(command.ProductId);
        savedSafetyDataSheet.FileName.Should().Be(command.FileName);
        savedSafetyDataSheet.FilePath.Should().Be(command.FilePath);
        savedSafetyDataSheet.Version.Should().Be(command.Version);
        savedSafetyDataSheet.EffectiveDate.Should().Be(command.EffectiveDate);
        savedSafetyDataSheet.UploadedAt.Should().Be(command.UploadedAt);
        savedSafetyDataSheet.UploadedByUserId.Should().Be(command.UploadedByUserId);
        savedSafetyDataSheet.IsActive.Should().BeTrue();

        await repo.Received(1)
            .AddAsync(savedSafetyDataSheet, Arg.Any<CancellationToken>());

        result.SafetyDataSheetId.Should().Be(savedSafetyDataSheet.SafetyDataSheetId);
        result.ProductId.Should().Be(command.ProductId);
        result.FileName.Should().Be(command.FileName);
        result.FilePath.Should().Be(command.FilePath);
        result.Version.Should().Be(command.Version);
        result.EffectiveDate.Should().Be(command.EffectiveDate);
        result.UploadedAt.Should().Be(command.UploadedAt);
        result.UploadedByUserId.Should().Be(command.UploadedByUserId);

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "SafetyDataSheet"),
            Arg.Is<int>(value => value == savedSafetyDataSheet.SafetyDataSheetId),
            Arg.Is<string>(value => value == "Added"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(command.FileName)),
            Arg.Any<CancellationToken>());
    }

    private static CreateSafetyDataSheetCommand CreateValidCommand()
    {
        return new CreateSafetyDataSheetCommand
        {
            ProductId = 123,
            FileName = "acetone-sds.pdf",
            FilePath = "/sds/acetone-sds.pdf",
            Version = "1.0",
            EffectiveDate = new DateTime(2026, 1, 1),
            UploadedAt = new DateTime(2026, 1, 2),
            UploadedByUserId = 4
        };
    }
}
