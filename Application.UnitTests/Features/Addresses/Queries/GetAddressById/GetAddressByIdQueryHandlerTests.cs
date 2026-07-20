using Application.Features.Addresses.Queries.GetAddressById;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Addresses.Queries.GetAddressById;

public class GetAddressByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAddress_ReturnsAddressDto()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var handler = new GetAddressByIdQueryHandler(repo);
        var address = CreateAddress();
        var query = new GetAddressByIdQuery
        {
            CustomerId = address.CustomerId!.Value,
            AddressId = address.AddressId
        };

        repo.GetByIdAsync(query.CustomerId, query.AddressId, Arg.Any<CancellationToken>())
            .Returns(address);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AddressId.Should().Be(address.AddressId);
        result.CustomerId.Should().Be(address.CustomerId);
        result.AddressType.Should().Be(address.AddressType);
        result.SiteName.Should().Be(address.SiteName);
        result.Line1.Should().Be(address.Line1);
        result.Line2.Should().Be(address.Line2);
        result.City.Should().Be(address.City);
        result.County.Should().Be(address.County);
        result.Postcode.Should().Be(address.Postcode);
        result.Country.Should().Be(address.Country);
        result.ContactName.Should().Be(address.ContactName);
        result.ContactPhone.Should().Be(address.ContactPhone);
        result.DeliveryInstructions.Should().Be(address.DeliveryInstructions);

        await repo.Received(1)
            .GetByIdAsync(query.CustomerId, query.AddressId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAddressDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var handler = new GetAddressByIdQueryHandler(repo);
        var query = new GetAddressByIdQuery
        {
            CustomerId = 123,
            AddressId = 456
        };

        repo.GetByIdAsync(query.CustomerId, query.AddressId, Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        await repo.Received(1)
            .GetByIdAsync(query.CustomerId, query.AddressId, Arg.Any<CancellationToken>());
    }

    private static Address CreateAddress()
    {
        return new Address
        {
            AddressId = 456,
            CustomerId = 123,
            AddressType = "DeliverySite",
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
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
