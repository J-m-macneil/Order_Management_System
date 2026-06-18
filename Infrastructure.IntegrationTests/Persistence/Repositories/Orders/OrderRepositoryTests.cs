using Domain.Entities.Orders;
using Domain.Entities.Status;
using Domain.Entities.Customers;
using Domain.Entities.Organisation;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Persistence.Repositories.Orders;

public class OrderRepositoryTests
{
    [Fact]
    public async Task GetPagedAsync_WhenFilteringFailed_ReturnsOrdersWithFailedStatusOrFailedJobs()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var repo = new OrderRepository(context);
        AddRequiredOrderReferences(context);

        var failedStatusOrder = CreateOrder("ORD-INT-FAILED-STATUS", 8);
        var failedJobOrder = CreateOrder("ORD-INT-FAILED-JOB", 3);
        var healthyOrder = CreateOrder("ORD-INT-HEALTHY", 3);

        context.Orders.AddRange(failedStatusOrder, failedJobOrder, healthyOrder);
        await context.SaveChangesAsync(CancellationToken.None);

        context.ProcessingJobs.Add(new ProcessingJob
        {
            OrderId = failedJobOrder.OrderId,
            JobType = "GenerateSdsBundle",
            Status = "Failed",
            AttemptCount = 1,
            MaxAttempts = 3,
            CreatedAt = DateTime.UtcNow,
            FailedAt = DateTime.UtcNow,
            ErrorMessage = "SDS generation failed"
        });
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var total = await repo.CountActiveAsync(
            searchTerm: null,
            orderStatusId: 8,
            isPriorityOrder: null,
            requestedDeliveryFrom: null,
            requestedDeliveryTo: null,
            createdFrom: null,
            createdTo: null,
            CancellationToken.None);

        var result = await repo.GetPagedAsync(
            skip: 0,
            take: 10,
            searchTerm: null,
            orderStatusId: 8,
            isPriorityOrder: null,
            requestedDeliveryFrom: null,
            requestedDeliveryTo: null,
            createdFrom: null,
            createdTo: null,
            CancellationToken.None);

        // Assert
        total.Should().Be(2);
        result.Select(x => x.OrderNumber)
            .Should()
            .BeEquivalentTo(new[] { failedStatusOrder.OrderNumber, failedJobOrder.OrderNumber });
        result.Should().Contain(x =>
            x.OrderNumber == failedJobOrder.OrderNumber &&
            x.ProcessingJobs.Any(j => j.Status == "Failed"));
        result.Should().NotContain(x => x.OrderNumber == healthyOrder.OrderNumber);
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

    private static Order CreateOrder(string orderNumber, int statusId)
    {
        return new Order
        {
            OrderNumber = orderNumber,
            CustomerId = 9001,
            DeliveryAddressId = 9002,
            BillingAddressId = 9001,
            CreatedByUserId = 1,
            WarehouseId = 9001,
            OrderStatusId = statusId,
            RequestedDeliveryDate = new DateTime(2026, 7, 1),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Currency = "GBP"
        };
    }

    private static void AddRequiredOrderReferences(AppDbContext context)
    {
        context.Addresses.AddRange(
            new Address
            {
                AddressId = 9001,
                AddressType = "Billing",
                SiteName = "Integration Billing",
                Line1 = "1 Test Street",
                City = "Manchester",
                Postcode = "M1 1AA",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Address
            {
                AddressId = 9002,
                AddressType = "DeliverySite",
                SiteName = "Integration Delivery",
                Line1 = "2 Test Street",
                City = "Manchester",
                Postcode = "M2 2AA",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Address
            {
                AddressId = 9003,
                AddressType = "Warehouse",
                SiteName = "Integration Warehouse",
                Line1 = "3 Test Street",
                City = "Manchester",
                Postcode = "M3 3AA",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

        context.Customers.Add(new Customer
        {
            CustomerId = 9001,
            AccountNumber = "INT-CUST-001",
            CompanyName = "Integration Customer",
            IndustryType = "Manufacturing",
            MainContactName = "Test User",
            MainContactEmail = "test@example.com",
            MainContactPhone = "01234567890",
            BillingAddressId = 9001,
            DefaultDeliveryAddressId = 9002,
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 1000m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        context.Warehouses.Add(new Warehouse
        {
            WarehouseId = 9001,
            Code = "INT-WH",
            Name = "Integration Warehouse",
            AddressId = 9003,
            IsActive = true
        });
    }
}
