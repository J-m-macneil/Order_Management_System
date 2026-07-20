using Application.Features.Addresses.Queries.GetCustomerAddresses;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Addresses.Queries.GetCustomerAddresses;

public class GetCustomerAddressesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithCustomerAddresses_ReturnsOrderedAddressDtos()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var handler = new GetCustomerAddressesQueryHandler(repo);
        var query = new GetCustomerAddressesQuery { CustomerId = 123 };
        var addresses = new List<Address>
        {
            CreateAddress(3, "DeliverySite", "Secondary Site"),
            CreateAddress(2, "Billing", "Billing Site"),
            CreateAddress(1, "DeliverySite", "Delivery Site A")
        };

        repo.GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(addresses);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Select(x => x.AddressId).Should().Equal(2, 1, 3);

        result[0].AddressType.Should().Be("Billing");

        result[1].SiteName.Should().Be("Delivery Site A");
        result[1].Line1.Should().Be("1 Test Street");
        result[1].City.Should().Be("Liverpool");
        result[1].Postcode.Should().Be("L1 1AA");
        result[1].ContactName.Should().Be("Jane Smith");
        result[1].DeliveryInstructions.Should().Be("Use side entrance");

        await repo.Received(1)
            .GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerHasNoAddresses_ReturnsEmptyList()
    {
        // Arrange
        var repo = Substitute.For<IAddressRepository>();
        var handler = new GetCustomerAddressesQueryHandler(repo);
        var query = new GetCustomerAddressesQuery { CustomerId = 123 };

        repo.GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(new List<Address>());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        await repo.Received(1)
            .GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>());
    }

    private static Address CreateAddress(
        int addressId,
        string addressType,
        string siteName)
    {
        return new Address
        {
            AddressId = addressId,
            CustomerId = 123,
            AddressType = addressType,
            SiteName = siteName,
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
