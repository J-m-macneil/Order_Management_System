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
        if (await _dbContext.Customers.AnyAsync())
        {
            return;
        }

        // =========================
        // 1. Create Customers
        // =========================
        var customer1 = new Customer
        {
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
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        var customer2 = new Customer
        {
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
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        await _dbContext.Customers.AddRangeAsync(customer1, customer2);
        await _dbContext.SaveChangesAsync();

        // =========================
        // 2. Create Customers
        // =========================

        var contacts = new List<CustomerContact>
        {
            new CustomerContact
            {
                CustomerId = 1,
                Name = "Oliver Evans",
                JobTitle = "Purchasing Manager",
                Email = "oliver.evans1@northwestsurface.co.uk",
                Phone = "07465341213",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new CustomerContact
            {
                CustomerId = 1,
                Name = "Luke Taylor",
                JobTitle = "Procurement Lead",
                Email = "luke.taylor1@northwestsurface.co.uk",
                Phone = "07331191390",
                IsPrimary = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },

            new CustomerContact
            {
                CustomerId = 2,
                Name = "Mia Wilson",
                JobTitle = "Purchasing Manager",
                Email = "mia.wilson2@merseywatersolut.co.uk",
                Phone = "07506448196",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new CustomerContact
            {
                CustomerId = 2,
                Name = "Jack Thomas",
                JobTitle = "Procurement Lead",
                Email = "jack.thomas2@merseywatersolut.co.uk",
                Phone = "07414797776",
                IsPrimary = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }

            // 👉 Add more from your dataset the same way
        };

        await _dbContext.CustomerContacts.AddRangeAsync(contacts);
        await _dbContext.SaveChangesAsync();


        // =========================
        // 3. Create Addresses
        // =========================
        var addresses = new List<Address>
        {
            // Customer 1
            new Address
            {
                CustomerId = customer1.CustomerId,
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
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Address
            {
                CustomerId = customer1.CustomerId,
                AddressType = "HeadOffice",
                SiteName = "NorthWest Surface Treatments Ltd Head Office",
                Line1 = "101 Liverpool Business Park",
                City = "Liverpool",
                County = "Merseyside",
                Postcode = "M2 201BB",
                Country = "United Kingdom",
                ContactName = "Main Reception",
                ContactPhone = "07128492780",
                IsPrimary = false,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Address
            {
                CustomerId = customer1.CustomerId,
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
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },

            // Customer 2
            new Address
            {
                CustomerId = customer2.CustomerId,
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
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Address
            {
                CustomerId = customer2.CustomerId,
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
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        await _dbContext.Addresses.AddRangeAsync(addresses);
        await _dbContext.SaveChangesAsync();

        // =========================
        // 4. Set Default Addresses
        // =========================
        var customer1Addresses = await _dbContext.Addresses
            .Where(x => x.CustomerId == customer1.CustomerId)
            .ToListAsync();

        var customer2Addresses = await _dbContext.Addresses
            .Where(x => x.CustomerId == customer2.CustomerId)
            .ToListAsync();

        customer1.BillingAddressId = customer1Addresses.First(x => x.AddressType == "Billing").AddressId;
        customer1.DefaultDeliveryAddressId = customer1Addresses.First(x => x.IsPrimary && x.AddressType == "DeliverySite").AddressId;

        customer2.BillingAddressId = customer2Addresses.First(x => x.AddressType == "Billing").AddressId;
        customer2.DefaultDeliveryAddressId = customer2Addresses.First(x => x.IsPrimary && x.AddressType == "DeliverySite").AddressId;

        await _dbContext.SaveChangesAsync();
    }
}