using Application.Features.Addresses.Commands.DeleteAddress;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Addresses.Commands.DeleteAddress;

public class DeleteAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAddress_DeletesAddressAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteAddressCommandHandler(repo, audit);
        var address = CreateExistingAddress();
        var command = new DeleteAddressCommand
        {
            CustomerId = address.CustomerId!.Value,
            AddressId = address.AddressId
        };

        repo.GetByIdAsync(command.CustomerId, command.AddressId, Arg.Any<CancellationToken>())
            .Returns(address);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        await repo.Received(1)
            .GetByIdAsync(command.CustomerId, command.AddressId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .DeleteAsync(address, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Address"),
            Arg.Is<int>(value => value == address.AddressId),
            Arg.Is<string>(value => value == "Deleted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(address.CustomerId!.Value.ToString()) &&
                message.Contains(address.SiteName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAddressDoesNotExist_ThrowsExceptionAndDoesNotDeleteAddress()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteAddressCommandHandler(repo, audit);
        var command = new DeleteAddressCommand
        {
            CustomerId = 123,
            AddressId = 456
        };

        repo.GetByIdAsync(command.CustomerId, command.AddressId, Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Address not found");

        await repo.Received(1)
            .GetByIdAsync(command.CustomerId, command.AddressId, Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .DeleteAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>());

        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    private static Address CreateExistingAddress()
    {
        return new Address
        {
            AddressId = 456,
            CustomerId = 123,
            AddressType = "Delivery",
            SiteName = "Main Warehouse",
            Line1 = "1 Test Street",
            Line2 = "Unit 4",
            City = "Liverpool",
            County = "Merseyside",
            Postcode = "L1 1AA",
            Country = "United Kingdom",
            ContactName = "Jane Smith",
            ContactPhone = "01234567890",
            DeliveryInstructions = "Use side entrance",
            IsPrimary = true,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1),
            DeletedAt = null
        };
    }
}
