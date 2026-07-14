using Application.Common.Exceptions;
using Application.Features.Products.Commands.DeleteSafetyDataSheet;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Commands.DeleteSafetyDataSheet;

public class DeleteSafetyDataSheetCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSafetyDataSheet_SoftDeletesSafetyDataSheetAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ISafetyDataSheetRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteSafetyDataSheetCommandHandler(repo, audit);
        var safetyDataSheet = CreateExistingSafetyDataSheet();
        var command = new DeleteSafetyDataSheetCommand
        {
            ProductId = safetyDataSheet.ProductId,
            SafetyDataSheetId = safetyDataSheet.SafetyDataSheetId
        };

        repo.GetByIdAsync(command.ProductId, command.SafetyDataSheetId, Arg.Any<CancellationToken>())
            .Returns(safetyDataSheet);

        // Act
        var before = DateTime.UtcNow;
        var result = await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        result.Should().Be(Unit.Value);
        safetyDataSheet.IsActive.Should().BeFalse();
        safetyDataSheet.DeletedAt.Should().NotBeNull();
        safetyDataSheet.DeletedAt!.Value.Should().BeOnOrAfter(before);
        safetyDataSheet.DeletedAt.Value.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .GetByIdAsync(command.ProductId, command.SafetyDataSheetId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "SafetyDataSheet"),
            Arg.Is<int>(value => value == safetyDataSheet.SafetyDataSheetId),
            Arg.Is<string>(value => value == "Deleted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(safetyDataSheet.FileName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSafetyDataSheetDoesNotExist_ThrowsExceptionAndDoesNotDelete()
    {
        // Arrange
        var repo = Substitute.For<ISafetyDataSheetRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteSafetyDataSheetCommandHandler(repo, audit);
        var command = new DeleteSafetyDataSheetCommand
        {
            ProductId = 123,
            SafetyDataSheetId = 456
        };

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
            FileName = "acetone-sds.pdf",
            FilePath = "/sds/acetone-sds.pdf",
            Version = "1.0",
            EffectiveDate = new DateTime(2026, 1, 1),
            UploadedAt = new DateTime(2026, 1, 2),
            UploadedByUserId = 4,
            IsActive = true,
            DeletedAt = null
        };
    }
}
