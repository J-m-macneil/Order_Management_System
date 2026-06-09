using Application.Features.Addresses.Commands.CreateAddress;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsAddressAndReturnsDto()
    {
        // Arrange
        Address? savedAddress = null;

        var repo = Substitute.For<IAddressRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new CreateAddressCommandHandler(repo, audit);
        var command = CreateValidCommand();

        repo.AddAsync(Arg.Do<Address>(address =>
        {
            address.AddressId = 456;
            savedAddress = address;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var before = DateTime.UtcNow;
        var result = await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        savedAddress.Should().NotBeNull();

        savedAddress!.AddressId.Should().Be(456);
        savedAddress.CustomerId.Should().Be(command.CustomerId);
        savedAddress.AddressType.Should().Be(command.AddressType);
        savedAddress.SiteName.Should().Be(command.SiteName);
        savedAddress.Line1.Should().Be(command.Line1);
        savedAddress.Line2.Should().Be(command.Line2);
        savedAddress.City.Should().Be(command.City);
        savedAddress.County.Should().Be(command.County);
        savedAddress.Postcode.Should().Be(command.Postcode);
        savedAddress.Country.Should().Be(command.Country);
        savedAddress.ContactName.Should().Be(command.ContactName);
        savedAddress.ContactPhone.Should().Be(command.ContactPhone);
        savedAddress.DeliveryInstructions.Should().Be(command.DeliveryInstructions);
        savedAddress.IsPrimary.Should().Be(command.IsPrimary);
        savedAddress.IsActive.Should().BeTrue();
        savedAddress.CreatedAt.Should().BeOnOrAfter(before);
        savedAddress.CreatedAt.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .AddAsync(savedAddress, Arg.Any<CancellationToken>());

        result.AddressId.Should().Be(savedAddress.AddressId);
        result.CustomerId.Should().Be(command.CustomerId);
        result.AddressType.Should().Be(command.AddressType);
        result.SiteName.Should().Be(command.SiteName);
        result.Line1.Should().Be(command.Line1);
        result.Line2.Should().Be(command.Line2);
        result.City.Should().Be(command.City);
        result.County.Should().Be(command.County);
        result.Postcode.Should().Be(command.Postcode);
        result.Country.Should().Be(command.Country);
        result.ContactName.Should().Be(command.ContactName);
        result.ContactPhone.Should().Be(command.ContactPhone);
        result.DeliveryInstructions.Should().Be(command.DeliveryInstructions);
        result.IsPrimary.Should().Be(command.IsPrimary);

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Address"),
            Arg.Is<int>(value => value == savedAddress.AddressId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(command.CustomerId.ToString()) &&
                message.Contains(command.SiteName)),
            Arg.Any<CancellationToken>());
    }

    private static CreateAddressCommand CreateValidCommand()
    {
        return new CreateAddressCommand
        {
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
            IsPrimary = true
        };
    }
}
