using Domain.Entities.Customers;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories;

public class CustomerRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WithExistingCustomer_ReturnsCustomerWithAddressesAndContacts()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerRepository(context);
        var customer = CreateCustomer("IT-CUST-001", "Integration Customer");

        context.Customers.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        var address = CreateAddress(customer.CustomerId);
        var contact = CreateContact(customer.CustomerId);

        context.Addresses.Add(address);
        context.CustomerContacts.Add(contact);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByIdAsync(customer.CustomerId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(customer.CustomerId);
        result.Addresses.Should().ContainSingle(x => x.AddressId == address.AddressId);
        result.Contacts.Should().ContainSingle(x => x.CustomerContactId == contact.CustomerContactId);
    }

    [Fact]
    public async Task GetPagedAsync_WithFilters_ReturnsMatchingCustomersInCompanyNameOrder()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerRepository(context);
        var searchTerm = $"Repository Filter {Guid.NewGuid():N}";

        var beta = CreateCustomer("IT-CUST-010", $"{searchTerm} Beta", isActive: true);
        var alpha = CreateCustomer("IT-CUST-011", $"{searchTerm} Alpha", isActive: true);
        var inactive = CreateCustomer("IT-CUST-012", $"{searchTerm} Inactive", isActive: false);
        var deleted = CreateCustomer("IT-CUST-013", $"{searchTerm} Deleted", isActive: true);
        deleted.DeletedAt = DateTime.UtcNow;

        context.Customers.AddRange(beta, alpha, inactive, deleted);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var total = await repo.CountActiveAsync(
            searchTerm,
            "Testing",
            30,
            true,
            CancellationToken.None);

        var result = await repo.GetPagedAsync(
            skip: 0,
            take: 10,
            searchTerm,
            industryType: "Testing",
            paymentTermsDays: 30,
            isActive: true,
            CancellationToken.None);

        // Assert
        total.Should().Be(2);
        result.Should().HaveCount(2);
        result.Select(x => x.CustomerId).Should().Equal(alpha.CustomerId, beta.CustomerId);
        result.Should().OnlyContain(x =>
            x.CompanyName.Contains(searchTerm) &&
            x.IndustryType == "Testing" &&
            x.PaymentTermsDays == 30 &&
            x.IsActive &&
            x.DeletedAt == null);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesCustomerAndExcludesCustomerFromGetById()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerRepository(context);
        var customer = CreateCustomer("IT-CUST-020", "Soft Delete Customer");

        context.Customers.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        await repo.DeleteAsync(customer, CancellationToken.None);

        var result = await repo.GetByIdAsync(customer.CustomerId, CancellationToken.None);

        // Assert
        customer.IsActive.Should().BeFalse();
        customer.DeletedAt.Should().NotBeNull();
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSummaryAsync_ReturnsCountsIncludingNewActiveAndInactiveCustomers()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerRepository(context);
        var before = await repo.GetSummaryAsync(CancellationToken.None);

        var active = CreateCustomer("IT-CUST-030", "Summary Active Customer", isActive: true);
        var inactive = CreateCustomer("IT-CUST-031", "Summary Inactive Customer", isActive: false);
        var deleted = CreateCustomer("IT-CUST-032", "Summary Deleted Customer", isActive: true);
        deleted.DeletedAt = DateTime.UtcNow;

        context.Customers.AddRange(active, inactive, deleted);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetSummaryAsync(CancellationToken.None);

        // Assert
        result.TotalCustomers.Should().Be(before.TotalCustomers + 2);
        result.ActiveCustomers.Should().Be(before.ActiveCustomers + 1);
        result.InactiveCustomers.Should().Be(before.InactiveCustomers + 1);
    }

    private static async Task<AppDbContext> CreateContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return context;
    }

    private static Customer CreateCustomer(
        string accountNumber,
        string companyName,
        bool isActive = true)
    {
        return new Customer
        {
            AccountNumber = accountNumber,
            CompanyName = companyName,
            IndustryType = "Testing",
            MainContactName = "Jane Smith",
            MainContactEmail = "jane@integration.test",
            MainContactPhone = "01234567890",
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Address CreateAddress(int customerId)
    {
        return new Address
        {
            CustomerId = customerId,
            AddressType = "Delivery",
            SiteName = "Integration Warehouse",
            Line1 = "1 Integration Street",
            City = "Liverpool",
            Postcode = "L1 1AA",
            Country = "United Kingdom",
            IsPrimary = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static CustomerContact CreateContact(int customerId)
    {
        return new CustomerContact
        {
            CustomerId = customerId,
            Name = "Jane Smith",
            JobTitle = "Purchasing Manager",
            Email = "jane@integration.test",
            Phone = "01234567890",
            IsPrimary = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
