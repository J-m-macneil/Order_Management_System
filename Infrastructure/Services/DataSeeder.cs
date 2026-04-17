using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DataSeeder : IDataSeeder
{
    private readonly AppDbContext _dbContext;

    public DataSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Customers.AnyAsync() || await _dbContext.Addresses.AnyAsync())
        {
            return;
        }

        var customers = new List<Customer>
        {
            new Customer
            {
                CustomerId = 1,
                AccountNumber = "CUST-2026-0001",
                CompanyName = "NorthWest Surface Treatments Ltd",
                IndustryType = "Manufacturing",
                MainContactName = "Sophie Murray",
                MainContactEmail = "purchasing1@northwestsurfacetr.co.uk",
                MainContactPhone = "07732719211",
                PricingTierId = 1,
                PaymentTermsDays = 45,
                CreditLimit = 40000.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 24, 0, 0, 0),
                DeletedAt = null
            },
            new Customer
            {
                CustomerId = 2,
                AccountNumber = "CUST-2026-0002",
                CompanyName = "Mersey Water Solutions",
                IndustryType = "Water Treatment",
                MainContactName = "Hannah Clark",
                MainContactEmail = "purchasing2@merseywatersolutio.co.uk",
                MainContactPhone = "07384027113",
                PricingTierId = 1,
                PaymentTermsDays = 30,
                CreditLimit = 15000.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 2, 2, 0, 0, 0),
                DeletedAt = null
            }
        };

        await _dbContext.Customers.AddRangeAsync(customers);
        await _dbContext.SaveChangesAsync();

        var addresses = new List<Address>
        {
            new Address
            {
                AddressId = 1,
                CustomerId = 1,
                AddressType = "Billing",
                SiteName = "NorthWest Surface Treatments Ltd Accounts",
                Line1 = "11 Commerce House",
                Line2 = "Industrial Estate",
                City = "Liverpool",
                County = "Merseyside",
                Postcode = "L2 101AA",
                Country = "United Kingdom",
                ContactName = "Accounts Payable",
                ContactPhone = "07746412689",
                DeliveryInstructions = null,
                IsPrimary = true
            },
            new Address
            {
                AddressId = 2,
                CustomerId = 1,
                AddressType = "HeadOffice",
                SiteName = "NorthWest Surface Treatments Ltd Head Office",
                Line1 = "101 Liverpool Business Park",
                Line2 = null,
                City = "Liverpool",
                County = "Merseyside",
                Postcode = "M2 201BB",
                Country = "United Kingdom",
                ContactName = "Main Reception",
                ContactPhone = "07128492780",
                DeliveryInstructions = null,
                IsPrimary = false
            },
            new Address
            {
                AddressId = 3,
                CustomerId = 1,
                AddressType = "DeliverySite",
                SiteName = "Site A",
                Line1 = "201 Distribution Road",
                Line2 = "Plant / Works",
                City = "Manchester",
                County = "Greater Manchester",
                Postcode = "W2 301CC",
                Country = "United Kingdom",
                ContactName = "Site Supervisor A",
                ContactPhone = "07702632297",
                DeliveryInstructions = "ADR driver PPE required on arrival.",
                IsPrimary = true
            },
            new Address
            {
                AddressId = 4,
                CustomerId = 1,
                AddressType = "DeliverySite",
                SiteName = "Site B",
                Line1 = "202 Distribution Road",
                Line2 = "Plant / Works",
                City = "Leeds",
                County = "Lancashire",
                Postcode = "W3 302CC",
                Country = "United Kingdom",
                ContactName = "Site Supervisor B",
                ContactPhone = "07868820204",
                DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.",
                IsPrimary = false
            },
            new Address
            {
                AddressId = 5,
                CustomerId = 2,
                AddressType = "Billing",
                SiteName = "Mersey Water Solutions Accounts",
                Line1 = "12 Commerce House",
                Line2 = "Industrial Estate",
                City = "Manchester",
                County = "Greater Manchester",
                Postcode = "L3 102AA",
                Country = "United Kingdom",
                ContactName = "Accounts Payable",
                ContactPhone = "07919795579",
                DeliveryInstructions = null,
                IsPrimary = true
            },
            new Address
            {
                AddressId = 6,
                CustomerId = 2,
                AddressType = "HeadOffice",
                SiteName = "Mersey Water Solutions Head Office",
                Line1 = "102 Manchester Business Park",
                Line2 = null,
                City = "Manchester",
                County = "Greater Manchester",
                Postcode = "M3 202BB",
                Country = "United Kingdom",
                ContactName = "Main Reception",
                ContactPhone = "07461415646",
                DeliveryInstructions = null,
                IsPrimary = false
            },
            new Address
            {
                AddressId = 7,
                CustomerId = 2,
                AddressType = "DeliverySite",
                SiteName = "Site A",
                Line1 = "202 Distribution Road",
                Line2 = "Plant / Works",
                City = "Leeds",
                County = "Lancashire",
                Postcode = "W3 302CC",
                Country = "United Kingdom",
                ContactName = "Site Supervisor A",
                ContactPhone = "07209747451",
                DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.",
                IsPrimary = true
            },
            new Address
            {
                AddressId = 8,
                CustomerId = 2,
                AddressType = "DeliverySite",
                SiteName = "Site B",
                Line1 = "203 Distribution Road",
                Line2 = "Plant / Works",
                City = "Warrington",
                County = "Cheshire",
                Postcode = "W4 303CC",
                Country = "United Kingdom",
                ContactName = "Site Supervisor B",
                ContactPhone = "07507943839",
                DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.",
                IsPrimary = false
            }
        };

        await _dbContext.Addresses.AddRangeAsync(addresses);
        await _dbContext.SaveChangesAsync();

        var customer1 = await _dbContext.Customers.FirstAsync(x => x.CustomerId == 1);
        customer1.BillingAddressId = 1;
        customer1.DefaultDeliveryAddressId = 3;

        var customer2 = await _dbContext.Customers.FirstAsync(x => x.CustomerId == 2);
        customer2.BillingAddressId = 5;
        customer2.DefaultDeliveryAddressId = 7;

        await _dbContext.SaveChangesAsync();
    }
}