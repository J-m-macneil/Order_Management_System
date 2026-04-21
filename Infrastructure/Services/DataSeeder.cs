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
        // =========================
        // 1. Customers
        // =========================
        if (!await _dbContext.Customers.AnyAsync())
        {
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
                CreatedAt = DateTime.UtcNow
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
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Customers.AddRangeAsync(customer1, customer2);
            await _dbContext.SaveChangesAsync();
        }

        var customers = await _dbContext.Customers.ToListAsync();

        // =========================
        // 2. Contacts
        // =========================
        if (!await _dbContext.CustomerContacts.AnyAsync())
        {
            var contacts = new List<CustomerContact>
        {
            new CustomerContact
            {
                CustomerId = customers[0].CustomerId,
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
                CustomerId = customers[0].CustomerId,
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
                CustomerId = customers[1].CustomerId,
                Name = "Mia Wilson",
                JobTitle = "Purchasing Manager",
                Email = "mia.wilson2@merseywatersolut.co.uk",
                Phone = "07506448196",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

            await _dbContext.CustomerContacts.AddRangeAsync(contacts);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // 3. Addresses
        // =========================
        if (!await _dbContext.Addresses.AnyAsync())
        {
            var addresses = new List<Address>
        {
            new Address
            {
                CustomerId = customers[0].CustomerId,
                AddressType = "Billing",
                SiteName = "Accounts",
                Line1 = "11 Commerce House",
                City = "Liverpool",
                Postcode = "L2 101AA",
                Country = "United Kingdom",
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Address
            {
                CustomerId = customers[0].CustomerId,
                AddressType = "DeliverySite",
                SiteName = "Site A",
                Line1 = "201 Distribution Road",
                City = "Manchester",
                Postcode = "W2 301CC",
                Country = "United Kingdom",
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

            await _dbContext.Addresses.AddRangeAsync(addresses);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // 4. Products ✅ NEW
        // =========================
        if (!await _dbContext.Products.AnyAsync())
        {
            var products = new List<Product>
        {
            new Product
            {
                SKU = "ACET-25",
                ProductName = "Acetone 25L Drum",
                Description = "Acetone 25L Drum supplied for industrial and commercial use.",
                ProductCategoryId = 1,
                UnitOfMeasureId = 3,
                PackSize = "25L",
                BasePrice = 85.00m,
                Currency = "GBP",
                HazardClassId = 2,
                UNNumber = "UN1090",
                StorageRequirement = "Flammable store",
                RequiresSds = true,
                IsRestricted = false,
                IsActive = true,
                CreatedAt = new DateTime(2024, 2, 7)
            },
            new Product
            {
                SKU = "IPA-20",
                ProductName = "Isopropyl Alcohol 99.9% 20L",
                Description = "Isopropyl Alcohol 20L supplied for industrial use.",
                ProductCategoryId = 1,
                UnitOfMeasureId = 3,
                PackSize = "20L",
                BasePrice = 92.50m,
                Currency = "GBP",
                HazardClassId = 2,
                UNNumber = "UN1219",
                StorageRequirement = "Flammable store",
                RequiresSds = true,
                IsRestricted = false,
                IsActive = true,
                CreatedAt = new DateTime(2024, 2, 13)
            }
        };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // 5. Customer Product Prices
        // =========================
        if (!await _dbContext.CustomerProductPrices.AnyAsync())
        {
            var items = new List<CustomerProductPrice>
    {
        new CustomerProductPrice
        {
            CustomerId = customers[1].CustomerId,
            ProductId = 2,
            OverridePrice = 90.18m,
            MinimumOrderQuantity = 2m,
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31),
            Notes = "Framework agreement",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }
    };

            await _dbContext.CustomerProductPrices.AddRangeAsync(items);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // 6. Safety Data Sheets ✅ NEW
        // =========================

        if (!await _dbContext.SafetyDataSheets.AnyAsync())
        {
            var items = new List<SafetyDataSheet>
            {
                new SafetyDataSheet
                {
                    ProductId = 1,
                    FileName = "acet_25_sds_v2.pdf",
                    FilePath = "/sds/acet_25_sds_v2.pdf",
                    Version = "V2",
                    EffectiveDate = new DateTime(2025, 11, 19),
                    UploadedAt = new DateTime(2026, 2, 12, 10, 0, 0),
                    UploadedByUserId = 8,
                    IsActive = true
                },
                new SafetyDataSheet
                {
                    ProductId = 2,
                    FileName = "ipa_20_sds_v2.pdf",
                    FilePath = "/sds/ipa_20_sds_v2.pdf",
                    Version = "V2",
                    EffectiveDate = new DateTime(2025, 7, 8),
                    UploadedAt = new DateTime(2026, 2, 11, 10, 0, 0),
                    UploadedByUserId = 1,
                    IsActive = true
                }
            };

            await _dbContext.SafetyDataSheets.AddRangeAsync(items);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // 7. Set Default Addresses
        // =========================
        var updatedCustomers = await _dbContext.Customers.ToListAsync();

        foreach (var customer in updatedCustomers)
        {
            var custAddresses = await _dbContext.Addresses
                .Where(x => x.CustomerId == customer.CustomerId)
                .ToListAsync();

            var billing = custAddresses.FirstOrDefault(x => x.AddressType == "Billing");
            var delivery = custAddresses.FirstOrDefault(x => x.AddressType == "DeliverySite" && x.IsPrimary);

            if (billing != null)
                customer.BillingAddressId = billing.AddressId;

            if (delivery != null)
                customer.DefaultDeliveryAddressId = delivery.AddressId;
        }

        await _dbContext.SaveChangesAsync();
    }
}