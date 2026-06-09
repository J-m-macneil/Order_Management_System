using Application.Features.Customers.Queries.GetCustomerContacts;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Queries.GetCustomerContacts;

public class GetCustomerContactsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithCustomerContacts_ReturnsContactDtos()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var handler = new GetCustomerContactsQueryHandler(repo);
        var query = new GetCustomerContactsQuery { CustomerId = 123 };
        var contacts = new List<CustomerContact>
        {
            CreateContact(456, "Jane Smith", isPrimary: true),
            CreateContact(789, "John Jones", isPrimary: false)
        };

        repo.GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(contacts);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);

        result[0].CustomerContactId.Should().Be(contacts[0].CustomerContactId);
        result[0].CustomerId.Should().Be(query.CustomerId);
        result[0].Name.Should().Be(contacts[0].Name);
        result[0].JobTitle.Should().Be(contacts[0].JobTitle);
        result[0].Email.Should().Be(contacts[0].Email);
        result[0].Phone.Should().Be(contacts[0].Phone);
        result[0].IsPrimary.Should().BeTrue();

        result[1].CustomerContactId.Should().Be(contacts[1].CustomerContactId);
        result[1].Name.Should().Be(contacts[1].Name);
        result[1].IsPrimary.Should().BeFalse();

        await repo.Received(1)
            .GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerHasNoContacts_ReturnsEmptyList()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var handler = new GetCustomerContactsQueryHandler(repo);
        var query = new GetCustomerContactsQuery { CustomerId = 123 };

        repo.GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(new List<CustomerContact>());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        await repo.Received(1)
            .GetByCustomerAsync(query.CustomerId, Arg.Any<CancellationToken>());
    }

    private static CustomerContact CreateContact(
        int customerContactId,
        string name,
        bool isPrimary)
    {
        return new CustomerContact
        {
            CustomerContactId = customerContactId,
            CustomerId = 123,
            Name = name,
            JobTitle = "Purchasing Manager",
            Email = $"{name.Replace(" ", ".").ToLowerInvariant()}@acme.test",
            Phone = "01234567890",
            IsPrimary = isPrimary,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
