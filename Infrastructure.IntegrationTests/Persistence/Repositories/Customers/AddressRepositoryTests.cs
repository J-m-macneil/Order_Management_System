using Domain.Entities.Customers;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Customers;

public class AddressRepositoryTests
{
    [Fact]
    public async Task GetByCustomerAsync_ReturnsOnlyActiveNonDeletedAddressesForCustomer()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new AddressRepository(context);
        var customer = CreateCustomer("ADDR-CUST-001");
        var otherCustomer = CreateCustomer("ADDR-CUST-002");

        context.Customers.AddRange(customer, otherCustomer);
        await context.SaveChangesAsync(CancellationToken.None);

        var activeAddress = CreateAddress(customer.CustomerId, "Active Site");
        var inactiveAddress = CreateAddress(customer.CustomerId, "Inactive Site");
        inactiveAddress.IsActive = false;
        var deletedAddress = CreateAddress(customer.CustomerId, "Deleted Site");
        deletedAddress.DeletedAt = DateTime.UtcNow;
        var otherCustomerAddress = CreateAddress(otherCustomer.CustomerId, "Other Customer Site");

        context.Addresses.AddRange(activeAddress, inactiveAddress, deletedAddress, otherCustomerAddress);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByCustomerAsync(customer.CustomerId, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].AddressId.Should().Be(activeAddress.AddressId);
        result[0].SiteName.Should().Be(activeAddress.SiteName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressBelongsToCustomerAndIsActive_ReturnsAddress()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new AddressRepository(context);
        var customer = CreateCustomer("ADDR-CUST-010");

        context.Customers.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        var address = CreateAddress(customer.CustomerId, "Lookup Site");
        context.Addresses.Add(address);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByIdAsync(
            customer.CustomerId,
            address.AddressId,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AddressId.Should().Be(address.AddressId);
        result.CustomerId.Should().Be(customer.CustomerId);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesAddressAndExcludesAddressFromQueries()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new AddressRepository(context);
        var customer = CreateCustomer("ADDR-CUST-020");

        context.Customers.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        var address = CreateAddress(customer.CustomerId, "Delete Site");
        context.Addresses.Add(address);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        await repo.DeleteAsync(address, CancellationToken.None);

        var result = await repo.GetByIdAsync(
            customer.CustomerId,
            address.AddressId,
            CancellationToken.None);

        // Assert
        address.IsActive.Should().BeFalse();
        address.DeletedAt.Should().NotBeNull();
        result.Should().BeNull();
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

    private static Customer CreateCustomer(string accountNumber)
    {
        return new Customer
        {
            AccountNumber = accountNumber,
            CompanyName = $"{accountNumber} Customer",
            IndustryType = "Testing",
            MainContactName = "Jane Smith",
            MainContactEmail = "jane@integration.test",
            MainContactPhone = "01234567890",
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Address CreateAddress(int customerId, string siteName)
    {
        return new Address
        {
            CustomerId = customerId,
            AddressType = "DeliverySite",
            SiteName = siteName,
            Line1 = "1 Integration Street",
            City = "Liverpool",
            Postcode = "L1 1AA",
            Country = "United Kingdom",
            IsPrimary = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
