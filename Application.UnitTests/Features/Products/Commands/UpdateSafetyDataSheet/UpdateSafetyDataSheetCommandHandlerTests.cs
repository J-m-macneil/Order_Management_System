using Application.Common.Exceptions;
using Application.Features.Products.Commands.UpdateSafetyDataSheet;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Commands.UpdateSafetyDataSheet;

public class UpdateSafetyDataSheetCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSafetyDataSheet_UpdatesSafetyDataSheetAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ISafetyDataSheetRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateSafetyDataSheetCommandHandler(repo, audit);
        var existingSafetyDataSheet = CreateExistingSafetyDataSheet();
        var command = CreateValidCommand();

        repo.GetByIdAsync(command.ProductId, command.SafetyDataSheetId, Arg.Any<CancellationToken>())
            .Returns(existingSafetyDataSheet);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        existingSafetyDataSheet.FileName.Should().Be(command.FileName);
        existingSafetyDataSheet.FilePath.Should().Be(command.FilePath);
        existingSafetyDataSheet.Version.Should().Be(command.Version);
        existingSafetyDataSheet.EffectiveDate.Should().Be(command.EffectiveDate);
        existingSafetyDataSheet.UploadedAt.Should().Be(command.UploadedAt);
        existingSafetyDataSheet.UploadedByUserId.Should().Be(command.UploadedByUserId);
        existingSafetyDataSheet.IsActive.Should().Be(command.IsActive);

        await repo.Received(1)
            .GetByIdAsync(command.ProductId, command.SafetyDataSheetId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "SafetyDataSheet"),
            Arg.Is<int>(value => value == existingSafetyDataSheet.SafetyDataSheetId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(command.FileName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSafetyDataSheetDoesNotExist_ThrowsExceptionAndDoesNotUpdate()
    {
        // Arrange
        var repo = Substitute.For<ISafetyDataSheetRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateSafetyDataSheetCommandHandler(repo, audit);
        var command = CreateValidCommand();

        repo.GetByIdAsync(command.ProductId, command.SafetyDataSheetId, Arg.Any<CancellationToken>())
            .Returns((SafetyDataSheet?)null);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Safety data sheet was not found.");

        await repo.Received(1)
            .GetByIdAsync(command.ProductId, command.SafetyDataSheetId, Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    private static SafetyDataSheet CreateExistingSafetyDataSheet()
    {
        return new SafetyDataSheet
        {
            SafetyDataSheetId = 456,
            ProductId = 123,
            FileName = "old-sds.pdf",
            FilePath = "/sds/old-sds.pdf",
            Version = "0.9",
            EffectiveDate = new DateTime(2025, 1, 1),
            UploadedAt = new DateTime(2025, 1, 2),
            UploadedByUserId = 3,
            IsActive = true
        };
    }

    private static UpdateSafetyDataSheetCommand CreateValidCommand()
    {
        return new UpdateSafetyDataSheetCommand
        {
            ProductId = 123,
            SafetyDataSheetId = 456,
            FileName = "acetone-sds.pdf",
            FilePath = "/sds/acetone-sds.pdf",
            Version = "1.0",
            EffectiveDate = new DateTime(2026, 1, 1),
            UploadedAt = new DateTime(2026, 1, 2),
            UploadedByUserId = 4,
            IsActive = true
        };
    }
}
