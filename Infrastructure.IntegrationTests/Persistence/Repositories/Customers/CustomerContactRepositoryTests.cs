using Domain.Entities.Customers;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Customers;

public class CustomerContactRepositoryTests
{
    [Fact]
    public async Task GetByCustomerAsync_ReturnsOnlyActiveNonDeletedContactsForCustomer()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerContactRepository(context);
        var customer = CreateCustomer("CONT-CUST-001");
        var otherCustomer = CreateCustomer("CONT-CUST-002");

        context.Customers.AddRange(customer, otherCustomer);
        await context.SaveChangesAsync(CancellationToken.None);

        var activeContact = CreateContact(customer.CustomerId, "Jane Smith", isPrimary: true);
        var inactiveContact = CreateContact(customer.CustomerId, "Inactive Contact", isPrimary: false);
        inactiveContact.IsActive = false;
        var deletedContact = CreateContact(customer.CustomerId, "Deleted Contact", isPrimary: false);
        deletedContact.DeletedAt = DateTime.UtcNow;
        var otherCustomerContact = CreateContact(otherCustomer.CustomerId, "Other Customer Contact", isPrimary: true);

        context.CustomerContacts.AddRange(activeContact, inactiveContact, deletedContact, otherCustomerContact);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repo.GetByCustomerAsync(customer.CustomerId, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].CustomerContactId.Should().Be(activeContact.CustomerContactId);
        result[0].Name.Should().Be(activeContact.Name);
    }

    [Fact]
    public async Task ClearPrimaryForCustomerAsync_ClearsPrimaryContactsExceptExcludedContact()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerContactRepository(context);
        var customer = CreateCustomer("CONT-CUST-010");
        var otherCustomer = CreateCustomer("CONT-CUST-011");

        context.Customers.AddRange(customer, otherCustomer);
        await context.SaveChangesAsync(CancellationToken.None);

        var excludedContact = CreateContact(customer.CustomerId, "Jane Smith", isPrimary: true);
        var contactToClear = CreateContact(customer.CustomerId, "John Jones", isPrimary: true);
        var inactiveContact = CreateContact(customer.CustomerId, "Inactive Contact", isPrimary: true);
        inactiveContact.IsActive = false;
        var otherCustomerContact = CreateContact(otherCustomer.CustomerId, "Other Customer Contact", isPrimary: true);

        context.CustomerContacts.AddRange(excludedContact, contactToClear, inactiveContact, otherCustomerContact);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        await repo.ClearPrimaryForCustomerAsync(
            customer.CustomerId,
            excludedContact.CustomerContactId,
            CancellationToken.None);

        await repo.SaveChangesAsync(CancellationToken.None);

        var contacts = await context.CustomerContacts
            .AsNoTracking()
            .ToListAsync(CancellationToken.None);

        // Assert
        contacts.Single(x => x.CustomerContactId == excludedContact.CustomerContactId)
            .IsPrimary.Should().BeTrue();

        contacts.Single(x => x.CustomerContactId == contactToClear.CustomerContactId)
            .IsPrimary.Should().BeFalse();

        contacts.Single(x => x.CustomerContactId == inactiveContact.CustomerContactId)
            .IsPrimary.Should().BeTrue();

        contacts.Single(x => x.CustomerContactId == otherCustomerContact.CustomerContactId)
            .IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_PersistsContact()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new CustomerContactRepository(context);
        var customer = CreateCustomer("CONT-CUST-020");

        context.Customers.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        var contact = CreateContact(customer.CustomerId, "Jane Smith", isPrimary: true);

        // Act
        await repo.AddAsync(contact, CancellationToken.None);

        var result = await context.CustomerContacts
            .AsNoTracking()
            .SingleAsync(x => x.CustomerContactId == contact.CustomerContactId, CancellationToken.None);

        // Assert
        result.CustomerId.Should().Be(customer.CustomerId);
        result.Name.Should().Be(contact.Name);
        result.IsPrimary.Should().BeTrue();
        result.IsActive.Should().BeTrue();
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

    private static CustomerContact CreateContact(
        int customerId,
        string name,
        bool isPrimary)
    {
        return new CustomerContact
        {
            CustomerId = customerId,
            Name = name,
            JobTitle = "Purchasing Manager",
            Email = $"{name.Replace(" ", ".").ToLowerInvariant()}@integration.test",
            Phone = "01234567890",
            IsPrimary = isPrimary,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
