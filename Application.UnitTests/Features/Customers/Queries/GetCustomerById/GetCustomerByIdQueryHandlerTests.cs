using Application.Features.Customers.Queries.GetCustomerById;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCustomer_ReturnsCustomerDto()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var handler = new GetCustomerByIdQueryHandler(repo);
        var customer = CreateCustomer();
        var query = new GetCustomerByIdQuery { CustomerId = customer.CustomerId };

        repo.GetByIdAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(customer.CustomerId);
        result.AccountNumber.Should().Be(customer.AccountNumber);
        result.CompanyName.Should().Be(customer.CompanyName);
        result.IndustryType.Should().Be(customer.IndustryType);
        result.MainContactName.Should().Be(customer.MainContactName);
        result.MainContactEmail.Should().Be(customer.MainContactEmail);
        result.MainContactPhone.Should().Be(customer.MainContactPhone);
        result.BillingAddressId.Should().Be(customer.BillingAddressId);
        result.DefaultDeliveryAddressId.Should().Be(customer.DefaultDeliveryAddressId);
        result.PricingTierId.Should().Be(customer.PricingTierId);
        result.PaymentTermsDays.Should().Be(customer.PaymentTermsDays);
        result.CreditLimit.Should().Be(customer.CreditLimit);
        result.IsActive.Should().Be(customer.IsActive);
        result.CreatedAt.Should().Be(customer.CreatedAt);

        await repo.Received(1)
            .GetByIdAsync(query.CustomerId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var handler = new GetCustomerByIdQueryHandler(repo);
        var query = new GetCustomerByIdQuery { CustomerId = 123 };

        repo.GetByIdAsync(query.CustomerId, Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        await repo.Received(1)
            .GetByIdAsync(query.CustomerId, Arg.Any<CancellationToken>());
    }

    private static Customer CreateCustomer()
    {
        return new Customer
        {
            CustomerId = 123,
            AccountNumber = "CUST-123",
            CompanyName = "Acme Chemicals",
            IndustryType = "Manufacturing",
            MainContactName = "Jane Smith",
            MainContactEmail = "jane@acme.test",
            MainContactPhone = "01234567890",
            BillingAddressId = 10,
            DefaultDeliveryAddressId = 11,
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
