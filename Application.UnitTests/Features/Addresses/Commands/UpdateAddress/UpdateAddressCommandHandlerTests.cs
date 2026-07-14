using Application.Features.Addresses.Commands.UpdateAddress;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAddress_UpdatesAddressAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateAddressCommandHanlder(repo, audit);
        var address = CreateExistingAddress();
        var command = CreateValidCommand();

        repo.GetByIdAsync(command.CustomerId, command.AddressId, Arg.Any<CancellationToken>())
            .Returns(address);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        address.AddressId.Should().Be(command.AddressId);
        address.CustomerId.Should().Be(command.CustomerId);
        address.AddressType.Should().Be(command.AddressType);
        address.SiteName.Should().Be(command.SiteName);
        address.Line1.Should().Be(command.Line1);
        address.Line2.Should().Be(command.Line2);
        address.City.Should().Be(command.City);
        address.County.Should().Be(command.County);
        address.Postcode.Should().Be(command.Postcode);
        address.Country.Should().Be(command.Country);
        address.ContactName.Should().Be(command.ContactName);
        address.ContactPhone.Should().Be(command.ContactPhone);
        address.DeliveryInstructions.Should().Be(command.DeliveryInstructions);
        address.IsPrimary.Should().Be(command.IsPrimary);

        await repo.Received(1)
            .GetByIdAsync(command.CustomerId, command.AddressId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Address"),
            Arg.Is<int>(value => value == address.AddressId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(command.CustomerId.ToString()) &&
                message.Contains(command.SiteName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAddressDoesNotExist_ThrowsExceptionAndDoesNotSaveChanges()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateAddressCommandHanlder(repo, audit);
        var command = CreateValidCommand();

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

    private static Address CreateExistingAddress()
    {
        return new Address
        {
            AddressId = 456,
            CustomerId = 123,
            AddressType = "Billing",
            SiteName = "Old Site",
            Line1 = "Old Line 1",
            Line2 = "Old Line 2",
            City = "Old City",
            County = "Old County",
            Postcode = "OLD 123",
            Country = "United Kingdom",
            ContactName = "Old Contact",
            ContactPhone = "00000000000",
            DeliveryInstructions = "Old instructions",
            IsPrimary = false,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }

    private static UpdateAddressCommand CreateValidCommand()
    {
        return new UpdateAddressCommand
        {
            CustomerId = 123,
            AddressId = 456,
            AddressType = "Delivery",
            SiteName = "Updated Warehouse",
            Line1 = "1 Updated Street",
            Line2 = "Unit 8",
            City = "Liverpool",
            County = "Merseyside",
            Postcode = "L2 2BB",
            Country = "United Kingdom",
            ContactName = "Jane Smith",
            ContactPhone = "01234567890",
            DeliveryInstructions = "Use loading bay",
            IsPrimary = true
        };
    }
}
