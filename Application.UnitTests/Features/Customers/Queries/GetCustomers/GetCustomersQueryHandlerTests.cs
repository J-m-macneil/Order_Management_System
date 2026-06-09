using Application.Features.Customers.Queries.GetCustomers;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithQuery_ReturnsPagedCustomerDtos()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var handler = new GetCustomersQueryHandler(repo);
        var query = new GetCustomersQuery
        {
            PageNumber = 2,
            PageSize = 10,
            SearchTerm = "Acme",
            IndustryType = "Manufacturing",
            PaymentTermsDays = 30,
            IsActive = true
        };

        var customers = new List<Customer>
        {
            CreateCustomer(11, "CUST-011", "Acme Chemicals"),
            CreateCustomer(12, "CUST-012", "Acme Manufacturing")
        };

        repo.CountActiveAsync(
                query.SearchTerm,
                query.IndustryType,
                query.PaymentTermsDays,
                query.IsActive,
                Arg.Any<CancellationToken>())
            .Returns(25);

        repo.GetPagedAsync(
                query.Skip,
                query.PageSize,
                query.SearchTerm,
                query.IndustryType,
                query.PaymentTermsDays,
                query.IsActive,
                Arg.Any<CancellationToken>())
            .Returns(customers);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(25);
        result.Items.Should().HaveCount(customers.Count);

        result.Items[0].CustomerId.Should().Be(customers[0].CustomerId);
        result.Items[0].AccountNumber.Should().Be(customers[0].AccountNumber);
        result.Items[0].CompanyName.Should().Be(customers[0].CompanyName);
        result.Items[0].IndustryType.Should().Be(customers[0].IndustryType);
        result.Items[0].MainContactName.Should().Be(customers[0].MainContactName);
        result.Items[0].MainContactEmail.Should().Be(customers[0].MainContactEmail);
        result.Items[0].MainContactPhone.Should().Be(customers[0].MainContactPhone);
        result.Items[0].BillingAddressId.Should().Be(customers[0].BillingAddressId);
        result.Items[0].DefaultDeliveryAddressId.Should().Be(customers[0].DefaultDeliveryAddressId);
        result.Items[0].PricingTierId.Should().Be(customers[0].PricingTierId);
        result.Items[0].PaymentTermsDays.Should().Be(customers[0].PaymentTermsDays);
        result.Items[0].CreditLimit.Should().Be(customers[0].CreditLimit);
        result.Items[0].IsActive.Should().Be(customers[0].IsActive);
        result.Items[0].CreatedAt.Should().Be(customers[0].CreatedAt);

        await repo.Received(1).CountActiveAsync(
            query.SearchTerm,
            query.IndustryType,
            query.PaymentTermsDays,
            query.IsActive,
            Arg.Any<CancellationToken>());

        await repo.Received(1).GetPagedAsync(
            query.Skip,
            query.PageSize,
            query.SearchTerm,
            query.IndustryType,
            query.PaymentTermsDays,
            query.IsActive,
            Arg.Any<CancellationToken>());
    }

    private static Customer CreateCustomer(
        int customerId,
        string accountNumber,
        string companyName)
    {
        return new Customer
        {
            CustomerId = customerId,
            AccountNumber = accountNumber,
            CompanyName = companyName,
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
