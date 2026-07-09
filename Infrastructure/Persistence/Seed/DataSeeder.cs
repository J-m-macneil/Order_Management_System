using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Customers;
using Domain.Entities.Orders;
using Domain.Entities.Organisation;
using Domain.Entities.Status;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

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
            var customersToSeed = new List<Customer>
            {
                new Customer
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
                    CreatedAt = new DateTime(2024, 1, 24)
                },
                new Customer
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
                    CreatedAt = new DateTime(2024, 2, 2)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0003",
                    CompanyName = "Alderley Analytical Labs",
                    IndustryType = "Laboratory Services",
                    MainContactName = "Ryan Davies",
                    MainContactEmail = "purchasing3@alderleyanalytical.co.uk",
                    MainContactPhone = "07995619255",
                    PricingTierId = 2,
                    PaymentTermsDays = 30,
                    CreditLimit = 40000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 2, 11)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0004",
                    CompanyName = "Pennine Industrial Cleaners",
                    IndustryType = "Industrial Cleaning",
                    MainContactName = "Ryan Murray",
                    MainContactEmail = "purchasing4@pennineindustrialc.co.uk",
                    MainContactPhone = "07389854268",
                    PricingTierId = 3,
                    PaymentTermsDays = 30,
                    CreditLimit = 100000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 2, 20)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0005",
                    CompanyName = "Redbrick Manufacturing Group",
                    IndustryType = "Food Processing",
                    MainContactName = "Hannah Wilson",
                    MainContactEmail = "purchasing5@redbrickmanufactur.co.uk",
                    MainContactPhone = "07803771909",
                    PricingTierId = 4,
                    PaymentTermsDays = 30,
                    CreditLimit = 50000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 2, 29)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0006",
                    CompanyName = "Atlantic Process Engineering",
                    IndustryType = "Automotive",
                    MainContactName = "Jack Wilson",
                    MainContactEmail = "purchasing6@atlanticprocesseng.co.uk",
                    MainContactPhone = "07773715057",
                    PricingTierId = 1,
                    PaymentTermsDays = 30,
                    CreditLimit = 25000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 3, 9)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0007",
                    CompanyName = "GreenCore Agriculture Supplies",
                    IndustryType = "Facilities Management",
                    MainContactName = "Ryan Brown",
                    MainContactEmail = "purchasing7@greencoreagricultu.co.uk",
                    MainContactPhone = "07875340444",
                    PricingTierId = 1,
                    PaymentTermsDays = 30,
                    CreditLimit = 100000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 3, 18)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0008",
                    CompanyName = "Sefton Facilities Services",
                    IndustryType = "Agriculture",
                    MainContactName = "Ryan Brown",
                    MainContactEmail = "purchasing8@seftonfacilitiesse.co.uk",
                    MainContactPhone = "07220117054",
                    PricingTierId = 2,
                    PaymentTermsDays = 30,
                    CreditLimit = 40000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 3, 27)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0009",
                    CompanyName = "BioChem Research Partners",
                    IndustryType = "Packaging",
                    MainContactName = "Oliver Taylor",
                    MainContactEmail = "purchasing9@biochemresearchpar.co.uk",
                    MainContactPhone = "07666585408",
                    PricingTierId = 5,
                    PaymentTermsDays = 60,
                    CreditLimit = 75000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 4, 5)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0010",
                    CompanyName = "Northern Food Processors Ltd",
                    IndustryType = "Process Engineering",
                    MainContactName = "Sophie Clark",
                    MainContactEmail = "purchasing10@northernfoodproces.co.uk",
                    MainContactPhone = "07336456621",
                    PricingTierId = 4,
                    PaymentTermsDays = 60,
                    CreditLimit = 15000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 4, 14)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0011",
                    CompanyName = "Riverline Packaging Solutions",
                    IndustryType = "Manufacturing",
                    MainContactName = "Oliver Davies",
                    MainContactEmail = "purchasing11@riverlinepackaging.co.uk",
                    MainContactPhone = "07713152854",
                    PricingTierId = 1,
                    PaymentTermsDays = 30,
                    CreditLimit = 75000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 4, 23)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0012",
                    CompanyName = "West Coast Engineering Chemicals",
                    IndustryType = "Water Treatment",
                    MainContactName = "Sophie Wilson",
                    MainContactEmail = "purchasing12@westcoastengineeri.co.uk",
                    MainContactPhone = "07304235259",
                    PricingTierId = 1,
                    PaymentTermsDays = 30,
                    CreditLimit = 75000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 5, 2)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0013",
                    CompanyName = "Liverpool Industrial Services",
                    IndustryType = "Laboratory Services",
                    MainContactName = "Sophie Murray",
                    MainContactEmail = "purchasing13@liverpoolindustria.co.uk",
                    MainContactPhone = "07162959284",
                    PricingTierId = 2,
                    PaymentTermsDays = 45,
                    CreditLimit = 25000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 5, 11)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0014",
                    CompanyName = "Delta Wastewater Management",
                    IndustryType = "Industrial Cleaning",
                    MainContactName = "Hannah Brown",
                    MainContactEmail = "purchasing14@deltawastewaterman.co.uk",
                    MainContactPhone = "07153839920",
                    PricingTierId = 3,
                    PaymentTermsDays = 30,
                    CreditLimit = 75000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 5, 20)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0015",
                    CompanyName = "Medilab Consumables UK",
                    IndustryType = "Food Processing",
                    MainContactName = "Grace Davies",
                    MainContactEmail = "purchasing15@medilabconsumables.co.uk",
                    MainContactPhone = "07726713347",
                    PricingTierId = 4,
                    PaymentTermsDays = 30,
                    CreditLimit = 75000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 5, 29)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0016",
                    CompanyName = "Crestline Automotive Coatings",
                    IndustryType = "Automotive",
                    MainContactName = "Jack Brown",
                    MainContactEmail = "purchasing16@crestlineautomotiv.co.uk",
                    MainContactPhone = "07677280546",
                    PricingTierId = 1,
                    PaymentTermsDays = 60,
                    CreditLimit = 25000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 6, 7)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0017",
                    CompanyName = "Ormskirk Process Controls",
                    IndustryType = "Facilities Management",
                    MainContactName = "Luke Davies",
                    MainContactEmail = "purchasing17@ormskirkprocesscon.co.uk",
                    MainContactPhone = "07211225158",
                    PricingTierId = 1,
                    PaymentTermsDays = 30,
                    CreditLimit = 25000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 6, 16)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0018",
                    CompanyName = "Harbour Facilities Management",
                    IndustryType = "Agriculture",
                    MainContactName = "Emma Brown",
                    MainContactEmail = "purchasing18@harbourfacilitiesm.co.uk",
                    MainContactPhone = "07781057736",
                    PricingTierId = 2,
                    PaymentTermsDays = 30,
                    CreditLimit = 50000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 6, 25)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0019",
                    CompanyName = "PrimeChem Blending Ltd",
                    IndustryType = "Packaging",
                    MainContactName = "Emma Taylor",
                    MainContactEmail = "purchasing19@primechemblending.co.uk",
                    MainContactPhone = "07725464884",
                    PricingTierId = 5,
                    PaymentTermsDays = 30,
                    CreditLimit = 75000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 7, 4)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0020",
                    CompanyName = "TerraNova Water Systems",
                    IndustryType = "Process Engineering",
                    MainContactName = "Oliver Wilson",
                    MainContactEmail = "purchasing20@terranovawatersyst.co.uk",
                    MainContactPhone = "07274484941",
                    PricingTierId = 5,
                    PaymentTermsDays = 45,
                    CreditLimit = 25000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 7, 13)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0021",
                    CompanyName = "Southport Maintenance Solutions",
                    IndustryType = "Manufacturing",
                    MainContactName = "Sophie Brown",
                    MainContactEmail = "purchasing21@southportmaintenan.co.uk",
                    MainContactPhone = "07808705238",
                    PricingTierId = 1,
                    PaymentTermsDays = 30,
                    CreditLimit = 25000.00m,
                    IsActive = false,
                    CreatedAt = new DateTime(2024, 7, 22)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0022",
                    CompanyName = "Grantham Packaging UK",
                    IndustryType = "Water Treatment",
                    MainContactName = "Grace Taylor",
                    MainContactEmail = "purchasing22@granthampackagingu.co.uk",
                    MainContactPhone = "07882269302",
                    PricingTierId = 1,
                    PaymentTermsDays = 60,
                    CreditLimit = 40000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 7, 31)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0023",
                    CompanyName = "Sterling Laboratory Products",
                    IndustryType = "Laboratory Services",
                    MainContactName = "Luke Clark",
                    MainContactEmail = "purchasing23@sterlinglaboratory.co.uk",
                    MainContactPhone = "07432091877",
                    PricingTierId = 2,
                    PaymentTermsDays = 30,
                    CreditLimit = 100000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 8, 9)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0024",
                    CompanyName = "Calder Industrial Services",
                    IndustryType = "Industrial Cleaning",
                    MainContactName = "Hannah Murray",
                    MainContactEmail = "purchasing24@calderindustrialse.co.uk",
                    MainContactPhone = "07574364121",
                    PricingTierId = 3,
                    PaymentTermsDays = 60,
                    CreditLimit = 50000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 8, 18)
                },
                new Customer
                {
                    AccountNumber = "CUST-2026-0025",
                    CompanyName = "Union Process Materials",
                    IndustryType = "Food Processing",
                    MainContactName = "Luke Wilson",
                    MainContactEmail = "purchasing25@unionprocessmateri.co.uk",
                    MainContactPhone = "07966040317",
                    PricingTierId = 4,
                    PaymentTermsDays = 30,
                    CreditLimit = 25000.00m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 8, 27)
                }
            };

            await _dbContext.Customers.AddRangeAsync(customersToSeed);
            await _dbContext.SaveChangesAsync();
        }

        var customers = await _dbContext.Customers
            .OrderBy(x => x.CustomerId)
            .ToListAsync();

        // =========================
        // 2. Contacts
        // =========================
        if (!await _dbContext.CustomerContacts.AnyAsync())
        {
            var contacts = new List<CustomerContact>
            {
                new CustomerContact { CustomerId = 1, Name = "Oliver Evans", JobTitle = "Purchasing Manager", Email = "oliver.evans1@northwestsurface.co.uk", Phone = "07465341213", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 1, Name = "Luke Taylor", JobTitle = "Procurement Lead", Email = "luke.taylor1@northwestsurface.co.uk", Phone = "07331191390", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 2, Name = "Mia Wilson", JobTitle = "Purchasing Manager", Email = "mia.wilson2@merseywatersolut.co.uk", Phone = "07506448196", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 2, Name = "Jack Thomas", JobTitle = "Procurement Lead", Email = "jack.thomas2@merseywatersolut.co.uk", Phone = "07414797776", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 2, Name = "Ruby Davies", JobTitle = "Lab Manager", Email = "ruby.davies2@merseywatersolut.co.uk", Phone = "07719927151", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 2, Name = "Sophie Wilson", JobTitle = "Operations Buyer", Email = "sophie.wilson2@merseywatersolut.co.uk", Phone = "07149203558", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 3, Name = "Hannah Davies", JobTitle = "Purchasing Manager", Email = "hannah.davies3@alderleyanalytic.co.uk", Phone = "07324956459", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 3, Name = "Ruby Clark", JobTitle = "Procurement Lead", Email = "ruby.clark3@alderleyanalytic.co.uk", Phone = "07853573823", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 4, Name = "Ruby Davies", JobTitle = "Purchasing Manager", Email = "ruby.davies4@pennineindustria.co.uk", Phone = "07924970419", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 4, Name = "Emma Murray", JobTitle = "Procurement Lead", Email = "emma.murray4@pennineindustria.co.uk", Phone = "07982403818", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 5, Name = "Ruby Roberts", JobTitle = "Purchasing Manager", Email = "ruby.roberts5@redbrickmanufact.co.uk", Phone = "07253407200", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 5, Name = "Luke Taylor", JobTitle = "Procurement Lead", Email = "luke.taylor5@redbrickmanufact.co.uk", Phone = "07364814270", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 5, Name = "Adam Thomas", JobTitle = "Lab Manager", Email = "adam.thomas5@redbrickmanufact.co.uk", Phone = "07678722458", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 5, Name = "Luke Evans", JobTitle = "Operations Buyer", Email = "luke.evans5@redbrickmanufact.co.uk", Phone = "07726563708", IsPrimary = false, IsActive = false, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 6, Name = "George Wilson", JobTitle = "Purchasing Manager", Email = "george.wilson6@atlanticprocesse.co.uk", Phone = "07513140753", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 6, Name = "Grace Roberts", JobTitle = "Procurement Lead", Email = "grace.roberts6@atlanticprocesse.co.uk", Phone = "07668132202", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 6, Name = "Luke Thomas", JobTitle = "Lab Manager", Email = "luke.thomas6@atlanticprocesse.co.uk", Phone = "07112327652", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 6, Name = "Ruby Wilson", JobTitle = "Operations Buyer", Email = "ruby.wilson6@atlanticprocesse.co.uk", Phone = "07831980933", IsPrimary = false, IsActive = false, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 7, Name = "Mia Taylor", JobTitle = "Purchasing Manager", Email = "mia.taylor7@greencoreagricul.co.uk", Phone = "07645119047", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 7, Name = "Jack Clark", JobTitle = "Procurement Lead", Email = "jack.clark7@greencoreagricul.co.uk", Phone = "07786066793", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 7, Name = "Mia Murray", JobTitle = "Lab Manager", Email = "mia.murray7@greencoreagricul.co.uk", Phone = "07264109919", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 8, Name = "Sophie Brown", JobTitle = "Purchasing Manager", Email = "sophie.brown8@seftonfacilities.co.uk", Phone = "07358633898", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 8, Name = "George Wilson", JobTitle = "Procurement Lead", Email = "george.wilson8@seftonfacilities.co.uk", Phone = "07191969690", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 8, Name = "Adam Roberts", JobTitle = "Lab Manager", Email = "adam.roberts8@seftonfacilities.co.uk", Phone = "07976198296", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 9, Name = "Sophie Thomas", JobTitle = "Purchasing Manager", Email = "sophie.thomas9@biochemresearchp.co.uk", Phone = "07910959828", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 9, Name = "Adam Murray", JobTitle = "Procurement Lead", Email = "adam.murray9@biochemresearchp.co.uk", Phone = "07865523129", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 9, Name = "Luke Evans", JobTitle = "Lab Manager", Email = "luke.evans9@biochemresearchp.co.uk", Phone = "07821218382", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 9, Name = "Ruby Davies", JobTitle = "Operations Buyer", Email = "ruby.davies9@biochemresearchp.co.uk", Phone = "07570406376", IsPrimary = false, IsActive = false, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 10, Name = "Adam Brown", JobTitle = "Purchasing Manager", Email = "adam.brown10@northernfoodproc.co.uk", Phone = "07345824373", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 10, Name = "Jack Brown", JobTitle = "Procurement Lead", Email = "jack.brown10@northernfoodproc.co.uk", Phone = "07454794895", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 11, Name = "Sophie Roberts", JobTitle = "Purchasing Manager", Email = "sophie.roberts11@riverlinepackagi.co.uk", Phone = "07967043303", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 11, Name = "Grace Murray", JobTitle = "Procurement Lead", Email = "grace.murray11@riverlinepackagi.co.uk", Phone = "07201281557", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 11, Name = "Jack Evans", JobTitle = "Lab Manager", Email = "jack.evans11@riverlinepackagi.co.uk", Phone = "07480424119", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 11, Name = "Grace Evans", JobTitle = "Operations Buyer", Email = "grace.evans11@riverlinepackagi.co.uk", Phone = "07601463916", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 12, Name = "Oliver Evans", JobTitle = "Purchasing Manager", Email = "oliver.evans12@westcoastenginee.co.uk", Phone = "07297018781", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 12, Name = "Luke Roberts", JobTitle = "Procurement Lead", Email = "luke.roberts12@westcoastenginee.co.uk", Phone = "07368227631", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 12, Name = "Jack Roberts", JobTitle = "Lab Manager", Email = "jack.roberts12@westcoastenginee.co.uk", Phone = "07967607278", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 12, Name = "Mia Wilson", JobTitle = "Operations Buyer", Email = "mia.wilson12@westcoastenginee.co.uk", Phone = "07154318806", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 13, Name = "Emma Evans", JobTitle = "Purchasing Manager", Email = "emma.evans13@liverpoolindustr.co.uk", Phone = "07384759615", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 13, Name = "Ryan Clark", JobTitle = "Procurement Lead", Email = "ryan.clark13@liverpoolindustr.co.uk", Phone = "07554200826", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 13, Name = "Adam Thomas", JobTitle = "Lab Manager", Email = "adam.thomas13@liverpoolindustr.co.uk", Phone = "07810678904", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 13, Name = "Adam Roberts", JobTitle = "Operations Buyer", Email = "adam.roberts13@liverpoolindustr.co.uk", Phone = "07266211829", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 14, Name = "Mia Thomas", JobTitle = "Purchasing Manager", Email = "mia.thomas14@deltawastewaterm.co.uk", Phone = "07269042105", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 14, Name = "Emma Thomas", JobTitle = "Procurement Lead", Email = "emma.thomas14@deltawastewaterm.co.uk", Phone = "07186019028", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 14, Name = "Oliver Wilson", JobTitle = "Lab Manager", Email = "oliver.wilson14@deltawastewaterm.co.uk", Phone = "07738914080", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 14, Name = "Jack Murray", JobTitle = "Operations Buyer", Email = "jack.murray14@deltawastewaterm.co.uk", Phone = "07533550699", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 15, Name = "Luke Murray", JobTitle = "Purchasing Manager", Email = "luke.murray15@medilabconsumabl.co.uk", Phone = "07819112790", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 15, Name = "Adam Davies", JobTitle = "Procurement Lead", Email = "adam.davies15@medilabconsumabl.co.uk", Phone = "07356287095", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 15, Name = "Luke Evans", JobTitle = "Lab Manager", Email = "luke.evans15@medilabconsumabl.co.uk", Phone = "07240529481", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 16, Name = "Oliver Davies", JobTitle = "Purchasing Manager", Email = "oliver.davies16@crestlineautomot.co.uk", Phone = "07173864104", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 16, Name = "Sophie Davies", JobTitle = "Procurement Lead", Email = "sophie.davies16@crestlineautomot.co.uk", Phone = "07406002884", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 16, Name = "Oliver Roberts", JobTitle = "Lab Manager", Email = "oliver.roberts16@crestlineautomot.co.uk", Phone = "07995226828", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 17, Name = "Jack Wilson", JobTitle = "Purchasing Manager", Email = "jack.wilson17@ormskirkprocessc.co.uk", Phone = "07897163846", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 17, Name = "Mia Taylor", JobTitle = "Procurement Lead", Email = "mia.taylor17@ormskirkprocessc.co.uk", Phone = "07392431670", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 17, Name = "Luke Murray", JobTitle = "Lab Manager", Email = "luke.murray17@ormskirkprocessc.co.uk", Phone = "07870530217", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 18, Name = "Emma Brown", JobTitle = "Purchasing Manager", Email = "emma.brown18@harbourfacilitie.co.uk", Phone = "07458153605", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 18, Name = "Oliver Clark", JobTitle = "Procurement Lead", Email = "oliver.clark18@harbourfacilitie.co.uk", Phone = "07273497327", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 18, Name = "Adam Roberts", JobTitle = "Lab Manager", Email = "adam.roberts18@harbourfacilitie.co.uk", Phone = "07692362342", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 19, Name = "Grace Taylor", JobTitle = "Purchasing Manager", Email = "grace.taylor19@primechemblendin.co.uk", Phone = "07144913399", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 19, Name = "Luke Davies", JobTitle = "Procurement Lead", Email = "luke.davies19@primechemblendin.co.uk", Phone = "07954829815", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 20, Name = "Emma Taylor", JobTitle = "Purchasing Manager", Email = "emma.taylor20@terranovawatersy.co.uk", Phone = "07890880074", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 20, Name = "Hannah Evans", JobTitle = "Procurement Lead", Email = "hannah.evans20@terranovawatersy.co.uk", Phone = "07961393416", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 20, Name = "Ruby Murray", JobTitle = "Lab Manager", Email = "ruby.murray20@terranovawatersy.co.uk", Phone = "07386480450", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 20, Name = "Oliver Wilson", JobTitle = "Operations Buyer", Email = "oliver.wilson20@terranovawatersy.co.uk", Phone = "07510751046", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 21, Name = "Hannah Clark", JobTitle = "Purchasing Manager", Email = "hannah.clark21@southportmainten.co.uk", Phone = "07174539964", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 21, Name = "Luke Davies", JobTitle = "Procurement Lead", Email = "luke.davies21@southportmainten.co.uk", Phone = "07788785773", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 21, Name = "Mia Evans", JobTitle = "Lab Manager", Email = "mia.evans21@southportmainten.co.uk", Phone = "07829627019", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 21, Name = "Mia Davies", JobTitle = "Operations Buyer", Email = "mia.davies21@southportmainten.co.uk", Phone = "07129635852", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 22, Name = "George Thomas", JobTitle = "Purchasing Manager", Email = "george.thomas22@granthampackagin.co.uk", Phone = "07224173718", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 22, Name = "Grace Murray", JobTitle = "Procurement Lead", Email = "grace.murray22@granthampackagin.co.uk", Phone = "07373506211", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 22, Name = "Emma Evans", JobTitle = "Lab Manager", Email = "emma.evans22@granthampackagin.co.uk", Phone = "07101815992", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 22, Name = "Mia Thomas", JobTitle = "Operations Buyer", Email = "mia.thomas22@granthampackagin.co.uk", Phone = "07837507241", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 23, Name = "Hannah Evans", JobTitle = "Purchasing Manager", Email = "hannah.evans23@sterlinglaborato.co.uk", Phone = "07848621833", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 23, Name = "Luke Thomas", JobTitle = "Procurement Lead", Email = "luke.thomas23@sterlinglaborato.co.uk", Phone = "07236674237", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 23, Name = "Sophie Evans", JobTitle = "Lab Manager", Email = "sophie.evans23@sterlinglaborato.co.uk", Phone = "07813962536", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 23, Name = "Grace Taylor", JobTitle = "Operations Buyer", Email = "grace.taylor23@sterlinglaborato.co.uk", Phone = "07760904107", IsPrimary = false, IsActive = false, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 24, Name = "Mia Roberts", JobTitle = "Purchasing Manager", Email = "mia.roberts24@calderindustrial.co.uk", Phone = "07952266893", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 24, Name = "Adam Taylor", JobTitle = "Procurement Lead", Email = "adam.taylor24@calderindustrial.co.uk", Phone = "07807435606", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow },

                new CustomerContact { CustomerId = 25, Name = "Emma Brown", JobTitle = "Purchasing Manager", Email = "emma.brown25@unionprocessmate.co.uk", Phone = "07362897676", IsPrimary = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CustomerContact { CustomerId = 25, Name = "Ryan Wilson", JobTitle = "Procurement Lead", Email = "ryan.wilson25@unionprocessmate.co.uk", Phone = "07588999385", IsPrimary = false, IsActive = true, CreatedAt = DateTime.UtcNow }
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
                new Address { CustomerId = 1, AddressType = "Billing", SiteName = "NorthWest Surface Treatments Ltd Accounts", Line1 = "11 Commerce House", Line2 = "Industrial Estate", City = "Liverpool", County = "Merseyside", Postcode = "L2 101AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07746412689", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 1, AddressType = "HeadOffice", SiteName = "NorthWest Surface Treatments Ltd Head Office", Line1 = "101 Liverpool Business Park", Line2 = null, City = "Liverpool", County = "Merseyside", Postcode = "M2 201BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07128492780", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 1, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "201 Distribution Road", Line2 = "Plant / Works", City = "Manchester", County = "Greater Manchester", Postcode = "W2 301CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07702632297", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 1, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "202 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "Lancashire", Postcode = "W3 302CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07868820204", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 2, AddressType = "Billing", SiteName = "Mersey Water Solutions Accounts", Line1 = "12 Commerce House", Line2 = "Industrial Estate", City = "Manchester", County = "Greater Manchester", Postcode = "L3 102AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07919795579", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 2, AddressType = "HeadOffice", SiteName = "Mersey Water Solutions Head Office", Line1 = "102 Manchester Business Park", Line2 = null, City = "Manchester", County = "Greater Manchester", Postcode = "M3 202BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07461415646", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 2, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "202 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "Lancashire", Postcode = "W3 302CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07209747451", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 2, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "203 Distribution Road", Line2 = "Plant / Works", City = "Warrington", County = "Cheshire", Postcode = "W4 303CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07507943839", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 3, AddressType = "Billing", SiteName = "Alderley Analytical Labs Accounts", Line1 = "13 Commerce House", Line2 = "Industrial Estate", City = "Leeds", County = "Lancashire", Postcode = "L4 103AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07930075810", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 3, AddressType = "HeadOffice", SiteName = "Alderley Analytical Labs Head Office", Line1 = "103 Leeds Business Park", Line2 = null, City = "Leeds", County = "Lancashire", Postcode = "M4 203BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07410727955", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 3, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "203 Distribution Road", Line2 = "Plant / Works", City = "Warrington", County = "Cheshire", Postcode = "W4 303CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07185675980", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 3, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "204 Distribution Road", Line2 = "Plant / Works", City = "St Helens", County = "West Yorkshire", Postcode = "W5 304CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07208449460", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 4, AddressType = "Billing", SiteName = "Pennine Industrial Cleaners Accounts", Line1 = "14 Commerce House", Line2 = "Industrial Estate", City = "Warrington", County = "Cheshire", Postcode = "L5 104AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07833953718", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 4, AddressType = "HeadOffice", SiteName = "Pennine Industrial Cleaners Head Office", Line1 = "104 Warrington Business Park", Line2 = null, City = "Warrington", County = "Cheshire", Postcode = "M5 204BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07795822698", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 4, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "204 Distribution Road", Line2 = "Plant / Works", City = "St Helens", County = "West Yorkshire", Postcode = "W5 304CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07176667861", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 4, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "205 Distribution Road", Line2 = "Plant / Works", City = "Preston", County = "Merseyside", Postcode = "W6 305CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07781802744", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 4, AddressType = "DeliverySite", SiteName = "Site C", Line1 = "206 Distribution Road", Line2 = "Plant / Works", City = "Southport", County = "Greater Manchester", Postcode = "W7 306CC", Country = "United Kingdom", ContactName = "Site Supervisor C", ContactPhone = "07673528321", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 5, AddressType = "Billing", SiteName = "Redbrick Manufacturing Group Accounts", Line1 = "15 Commerce House", Line2 = "Industrial Estate", City = "St Helens", County = "West Yorkshire", Postcode = "L6 105AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07134467368", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 5, AddressType = "HeadOffice", SiteName = "Redbrick Manufacturing Group Head Office", Line1 = "105 St Helens Business Park", Line2 = null, City = "St Helens", County = "West Yorkshire", Postcode = "M6 205BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07964411347", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 5, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "205 Distribution Road", Line2 = "Plant / Works", City = "Preston", County = "Merseyside", Postcode = "W6 305CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07438715135", DeliveryInstructions = "Forklift available on request.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 5, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "206 Distribution Road", Line2 = "Plant / Works", City = "Southport", County = "Greater Manchester", Postcode = "W7 306CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07387484583", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 6, AddressType = "Billing", SiteName = "Atlantic Process Engineering Accounts", Line1 = "16 Commerce House", Line2 = "Industrial Estate", City = "Preston", County = "Merseyside", Postcode = "L7 106AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07488690725", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 6, AddressType = "HeadOffice", SiteName = "Atlantic Process Engineering Head Office", Line1 = "106 Preston Business Park", Line2 = null, City = "Preston", County = "Merseyside", Postcode = "M7 206BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07335493870", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 6, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "206 Distribution Road", Line2 = "Plant / Works", City = "Southport", County = "Greater Manchester", Postcode = "W7 306CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07248532577", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 6, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "207 Distribution Road", Line2 = "Plant / Works", City = "Bolton", County = "Lancashire", Postcode = "W8 307CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07629908599", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 7, AddressType = "Billing", SiteName = "GreenCore Agriculture Supplies Accounts", Line1 = "17 Commerce House", Line2 = "Industrial Estate", City = "Southport", County = "Greater Manchester", Postcode = "L8 107AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07925276600", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 7, AddressType = "HeadOffice", SiteName = "GreenCore Agriculture Supplies Head Office", Line1 = "107 Southport Business Park", Line2 = null, City = "Southport", County = "Greater Manchester", Postcode = "M8 207BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07788227492", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 7, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "207 Distribution Road", Line2 = "Plant / Works", City = "Bolton", County = "Lancashire", Postcode = "W8 307CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07465260635", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 7, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "208 Distribution Road", Line2 = "Plant / Works", City = "Chester", County = "Cheshire", Postcode = "W9 308CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07415143362", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 8, AddressType = "Billing", SiteName = "Sefton Facilities Services Accounts", Line1 = "18 Commerce House", Line2 = "Industrial Estate", City = "Bolton", County = "Lancashire", Postcode = "L9 108AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07501486939", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 8, AddressType = "HeadOffice", SiteName = "Sefton Facilities Services Head Office", Line1 = "108 Bolton Business Park", Line2 = null, City = "Bolton", County = "Lancashire", Postcode = "M1 208BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07918739736", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 8, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "208 Distribution Road", Line2 = "Plant / Works", City = "Chester", County = "Cheshire", Postcode = "W9 308CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07273461957", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 8, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "209 Distribution Road", Line2 = "Plant / Works", City = "Runcorn", County = "West Yorkshire", Postcode = "W1 309CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07936043811", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 8, AddressType = "DeliverySite", SiteName = "Site C", Line1 = "210 Distribution Road", Line2 = "Plant / Works", City = "Widnes", County = "Merseyside", Postcode = "W2 310CC", Country = "United Kingdom", ContactName = "Site Supervisor C", ContactPhone = "07100614068", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 9, AddressType = "Billing", SiteName = "BioChem Research Partners Accounts", Line1 = "19 Commerce House", Line2 = "Industrial Estate", City = "Chester", County = "Cheshire", Postcode = "L1 109AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07174316370", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 9, AddressType = "HeadOffice", SiteName = "BioChem Research Partners Head Office", Line1 = "109 Chester Business Park", Line2 = null, City = "Chester", County = "Cheshire", Postcode = "M2 209BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07916690353", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 9, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "209 Distribution Road", Line2 = "Plant / Works", City = "Runcorn", County = "West Yorkshire", Postcode = "W1 309CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07671988889", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 9, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "210 Distribution Road", Line2 = "Plant / Works", City = "Widnes", County = "Merseyside", Postcode = "W2 310CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07237859287", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 10, AddressType = "Billing", SiteName = "Northern Food Processors Ltd Accounts", Line1 = "20 Commerce House", Line2 = "Industrial Estate", City = "Runcorn", County = "West Yorkshire", Postcode = "L2 110AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07229927269", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 10, AddressType = "HeadOffice", SiteName = "Northern Food Processors Ltd Head Office", Line1 = "110 Runcorn Business Park", Line2 = null, City = "Runcorn", County = "West Yorkshire", Postcode = "M3 210BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07366186631", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 10, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "210 Distribution Road", Line2 = "Plant / Works", City = "Widnes", County = "Merseyside", Postcode = "W2 310CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07341266931", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 10, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "211 Distribution Road", Line2 = "Plant / Works", City = "Blackburn", County = "Greater Manchester", Postcode = "W3 311CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07463016614", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 11, AddressType = "Billing", SiteName = "Riverline Packaging Solutions Accounts", Line1 = "21 Commerce House", Line2 = "Industrial Estate", City = "Widnes", County = "Merseyside", Postcode = "L3 111AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07176082500", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 11, AddressType = "HeadOffice", SiteName = "Riverline Packaging Solutions Head Office", Line1 = "111 Widnes Business Park", Line2 = null, City = "Widnes", County = "Merseyside", Postcode = "M4 211BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07652070932", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 11, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "211 Distribution Road", Line2 = "Plant / Works", City = "Blackburn", County = "Greater Manchester", Postcode = "W3 311CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07355555531", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 11, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "212 Distribution Road", Line2 = "Plant / Works", City = "Liverpool", County = "Lancashire", Postcode = "W4 312CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07818309417", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 12, AddressType = "Billing", SiteName = "West Coast Engineering Chemicals Accounts", Line1 = "22 Commerce House", Line2 = "Industrial Estate", City = "Blackburn", County = "Greater Manchester", Postcode = "L4 112AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07823019672", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 12, AddressType = "HeadOffice", SiteName = "West Coast Engineering Chemicals Head Office", Line1 = "112 Blackburn Business Park", Line2 = null, City = "Blackburn", County = "Greater Manchester", Postcode = "M5 212BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07801642483", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 12, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "212 Distribution Road", Line2 = "Plant / Works", City = "Liverpool", County = "Lancashire", Postcode = "W4 312CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07793830224", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 12, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "213 Distribution Road", Line2 = "Plant / Works", City = "Manchester", County = "Cheshire", Postcode = "W5 313CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07165082363", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 12, AddressType = "DeliverySite", SiteName = "Site C", Line1 = "214 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "West Yorkshire", Postcode = "W6 314CC", Country = "United Kingdom", ContactName = "Site Supervisor C", ContactPhone = "07881913386", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 13, AddressType = "Billing", SiteName = "Liverpool Industrial Services Accounts", Line1 = "23 Commerce House", Line2 = "Industrial Estate", City = "Liverpool", County = "Lancashire", Postcode = "L5 113AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07200140141", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 13, AddressType = "HeadOffice", SiteName = "Liverpool Industrial Services Head Office", Line1 = "113 Liverpool Business Park", Line2 = null, City = "Liverpool", County = "Lancashire", Postcode = "M6 213BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07909134796", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 13, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "213 Distribution Road", Line2 = "Plant / Works", City = "Manchester", County = "Cheshire", Postcode = "W5 313CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07353810544", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 13, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "214 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "West Yorkshire", Postcode = "W6 314CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07536383774", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 14, AddressType = "Billing", SiteName = "Delta Wastewater Management Accounts", Line1 = "24 Commerce House", Line2 = "Industrial Estate", City = "Manchester", County = "Cheshire", Postcode = "L6 114AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07418587604", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 14, AddressType = "HeadOffice", SiteName = "Delta Wastewater Management Head Office", Line1 = "114 Manchester Business Park", Line2 = null, City = "Manchester", County = "Cheshire", Postcode = "M7 214BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07333754555", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 14, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "214 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "West Yorkshire", Postcode = "W6 314CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07162795957", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 14, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "215 Distribution Road", Line2 = "Plant / Works", City = "Warrington", County = "Merseyside", Postcode = "W7 315CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07889991777", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 15, AddressType = "Billing", SiteName = "Medilab Consumables UK Accounts", Line1 = "25 Commerce House", Line2 = "Industrial Estate", City = "Leeds", County = "West Yorkshire", Postcode = "L7 115AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07711684318", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 15, AddressType = "HeadOffice", SiteName = "Medilab Consumables UK Head Office", Line1 = "115 Leeds Business Park", Line2 = null, City = "Leeds", County = "West Yorkshire", Postcode = "M8 215BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07364371717", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 15, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "215 Distribution Road", Line2 = "Plant / Works", City = "Warrington", County = "Merseyside", Postcode = "W7 315CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07721609600", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 15, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "216 Distribution Road", Line2 = "Plant / Works", City = "St Helens", County = "Greater Manchester", Postcode = "W8 316CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07142672287", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 16, AddressType = "Billing", SiteName = "Crestline Automotive Coatings Accounts", Line1 = "26 Commerce House", Line2 = "Industrial Estate", City = "Warrington", County = "Merseyside", Postcode = "L8 116AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07821221887", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 16, AddressType = "HeadOffice", SiteName = "Crestline Automotive Coatings Head Office", Line1 = "116 Warrington Business Park", Line2 = null, City = "Warrington", County = "Merseyside", Postcode = "M1 216BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07793101194", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 16, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "216 Distribution Road", Line2 = "Plant / Works", City = "St Helens", County = "Greater Manchester", Postcode = "W8 316CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07422119408", DeliveryInstructions = "Forklift available on request.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 16, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "217 Distribution Road", Line2 = "Plant / Works", City = "Preston", County = "Lancashire", Postcode = "W9 317CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07439492674", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 16, AddressType = "DeliverySite", SiteName = "Site C", Line1 = "218 Distribution Road", Line2 = "Plant / Works", City = "Southport", County = "Cheshire", Postcode = "W1 318CC", Country = "United Kingdom", ContactName = "Site Supervisor C", ContactPhone = "07110002396", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 17, AddressType = "Billing", SiteName = "Ormskirk Process Controls Accounts", Line1 = "27 Commerce House", Line2 = "Industrial Estate", City = "St Helens", County = "Greater Manchester", Postcode = "L9 117AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07683275843", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 17, AddressType = "HeadOffice", SiteName = "Ormskirk Process Controls Head Office", Line1 = "117 St Helens Business Park", Line2 = null, City = "St Helens", County = "Greater Manchester", Postcode = "M2 217BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07855420240", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 17, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "217 Distribution Road", Line2 = "Plant / Works", City = "Preston", County = "Lancashire", Postcode = "W9 317CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07424825305", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 17, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "218 Distribution Road", Line2 = "Plant / Works", City = "Southport", County = "Cheshire", Postcode = "W1 318CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07966616964", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 18, AddressType = "Billing", SiteName = "Harbour Facilities Management Accounts", Line1 = "28 Commerce House", Line2 = "Industrial Estate", City = "Preston", County = "Lancashire", Postcode = "L1 118AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07468164906", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 18, AddressType = "HeadOffice", SiteName = "Harbour Facilities Management Head Office", Line1 = "118 Preston Business Park", Line2 = null, City = "Preston", County = "Lancashire", Postcode = "M3 218BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07318610946", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 18, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "218 Distribution Road", Line2 = "Plant / Works", City = "Southport", County = "Cheshire", Postcode = "W1 318CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07838194420", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 18, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "219 Distribution Road", Line2 = "Plant / Works", City = "Bolton", County = "West Yorkshire", Postcode = "W2 319CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07642678844", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 19, AddressType = "Billing", SiteName = "PrimeChem Blending Ltd Accounts", Line1 = "29 Commerce House", Line2 = "Industrial Estate", City = "Southport", County = "Cheshire", Postcode = "L2 119AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07857704655", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 19, AddressType = "HeadOffice", SiteName = "PrimeChem Blending Ltd Head Office", Line1 = "119 Southport Business Park", Line2 = null, City = "Southport", County = "Cheshire", Postcode = "M4 219BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07559225330", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 19, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "219 Distribution Road", Line2 = "Plant / Works", City = "Bolton", County = "West Yorkshire", Postcode = "W2 319CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07702269164", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 19, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "220 Distribution Road", Line2 = "Plant / Works", City = "Chester", County = "Merseyside", Postcode = "W3 320CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07220123666", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 20, AddressType = "Billing", SiteName = "TerraNova Water Systems Accounts", Line1 = "30 Commerce House", Line2 = "Industrial Estate", City = "Bolton", County = "West Yorkshire", Postcode = "L3 120AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07142836793", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 20, AddressType = "HeadOffice", SiteName = "TerraNova Water Systems Head Office", Line1 = "120 Bolton Business Park", Line2 = null, City = "Bolton", County = "West Yorkshire", Postcode = "M5 220BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07484194737", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 20, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "220 Distribution Road", Line2 = "Plant / Works", City = "Chester", County = "Merseyside", Postcode = "W3 320CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07325567963", DeliveryInstructions = "ADR driver PPE required on arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 20, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "221 Distribution Road", Line2 = "Plant / Works", City = "Runcorn", County = "Greater Manchester", Postcode = "W4 321CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07816114302", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 20, AddressType = "DeliverySite", SiteName = "Site C", Line1 = "222 Distribution Road", Line2 = "Plant / Works", City = "Widnes", County = "Lancashire", Postcode = "W5 322CC", Country = "United Kingdom", ContactName = "Site Supervisor C", ContactPhone = "07479759538", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 21, AddressType = "Billing", SiteName = "Southport Maintenance Solutions Accounts", Line1 = "31 Commerce House", Line2 = "Industrial Estate", City = "Chester", County = "Merseyside", Postcode = "L4 121AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07605399561", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 21, AddressType = "HeadOffice", SiteName = "Southport Maintenance Solutions Head Office", Line1 = "121 Chester Business Park", Line2 = null, City = "Chester", County = "Merseyside", Postcode = "M6 221BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07338836384", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 21, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "221 Distribution Road", Line2 = "Plant / Works", City = "Runcorn", County = "Greater Manchester", Postcode = "W4 321CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07314289692", DeliveryInstructions = "Forklift available on request.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 21, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "222 Distribution Road", Line2 = "Plant / Works", City = "Widnes", County = "Lancashire", Postcode = "W5 322CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07475442872", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 22, AddressType = "Billing", SiteName = "Grantham Packaging UK Accounts", Line1 = "32 Commerce House", Line2 = "Industrial Estate", City = "Runcorn", County = "Greater Manchester", Postcode = "L5 122AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07380477683", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 22, AddressType = "HeadOffice", SiteName = "Grantham Packaging UK Head Office", Line1 = "122 Runcorn Business Park", Line2 = null, City = "Runcorn", County = "Greater Manchester", Postcode = "M7 222BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07291735734", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 22, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "222 Distribution Road", Line2 = "Plant / Works", City = "Widnes", County = "Lancashire", Postcode = "W5 322CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07723403479", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 22, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "223 Distribution Road", Line2 = "Plant / Works", City = "Blackburn", County = "Cheshire", Postcode = "W6 323CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07141078352", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 23, AddressType = "Billing", SiteName = "Sterling Laboratory Products Accounts", Line1 = "33 Commerce House", Line2 = "Industrial Estate", City = "Widnes", County = "Lancashire", Postcode = "L6 123AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07491079829", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 23, AddressType = "HeadOffice", SiteName = "Sterling Laboratory Products Head Office", Line1 = "123 Widnes Business Park", Line2 = null, City = "Widnes", County = "Lancashire", Postcode = "M8 223BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07563102056", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 23, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "223 Distribution Road", Line2 = "Plant / Works", City = "Blackburn", County = "Cheshire", Postcode = "W6 323CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07175133802", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 23, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "224 Distribution Road", Line2 = "Plant / Works", City = "Liverpool", County = "West Yorkshire", Postcode = "W7 324CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07769108025", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 24, AddressType = "Billing", SiteName = "Calder Industrial Services Accounts", Line1 = "34 Commerce House", Line2 = "Industrial Estate", City = "Blackburn", County = "Cheshire", Postcode = "L7 124AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07536019893", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 24, AddressType = "HeadOffice", SiteName = "Calder Industrial Services Head Office", Line1 = "124 Blackburn Business Park", Line2 = null, City = "Blackburn", County = "Cheshire", Postcode = "M1 224BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07688343096", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 24, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "224 Distribution Road", Line2 = "Plant / Works", City = "Liverpool", County = "West Yorkshire", Postcode = "W7 324CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07995205588", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 24, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "225 Distribution Road", Line2 = "Plant / Works", City = "Manchester", County = "Merseyside", Postcode = "W8 325CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07426283245", DeliveryInstructions = "Call site contact 30 minutes before arrival.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 24, AddressType = "DeliverySite", SiteName = "Site C", Line1 = "226 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "Greater Manchester", Postcode = "W9 326CC", Country = "United Kingdom", ContactName = "Site Supervisor C", ContactPhone = "07325681936", DeliveryInstructions = "Forklift available on request.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = 25, AddressType = "Billing", SiteName = "Union Process Materials Accounts", Line1 = "35 Commerce House", Line2 = "Industrial Estate", City = "Liverpool", County = "West Yorkshire", Postcode = "L8 125AA", Country = "United Kingdom", ContactName = "Accounts Payable", ContactPhone = "07191048514", DeliveryInstructions = null, IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 25, AddressType = "HeadOffice", SiteName = "Union Process Materials Head Office", Line1 = "125 Liverpool Business Park", Line2 = null, City = "Liverpool", County = "West Yorkshire", Postcode = "M2 225BB", Country = "United Kingdom", ContactName = "Main Reception", ContactPhone = "07404713332", DeliveryInstructions = null, IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 25, AddressType = "DeliverySite", SiteName = "Site A", Line1 = "225 Distribution Road", Line2 = "Plant / Works", City = "Manchester", County = "Merseyside", Postcode = "W8 325CC", Country = "United Kingdom", ContactName = "Site Supervisor A", ContactPhone = "07653462378", DeliveryInstructions = "Use rear loading bay and present PO at gatehouse.", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = 25, AddressType = "DeliverySite", SiteName = "Site B", Line1 = "226 Distribution Road", Line2 = "Plant / Works", City = "Leeds", County = "Greater Manchester", Postcode = "W9 326CC", Country = "United Kingdom", ContactName = "Site Supervisor B", ContactPhone = "07459905654", DeliveryInstructions = "Deliver to goods-in between 07:30 and 15:00.", IsPrimary = false, CreatedAt = DateTime.UtcNow, IsActive = true },

                new Address { CustomerId = null, AddressType = "WarehousePartner", SiteName = "Runcorn Central Warehouse", Line1 = "401 Logistics Way", Line2 = null, City = "Runcorn", County = "Cheshire", Postcode = "WA7 4AA", Country = "United Kingdom", ContactName = "Nathan Price", ContactPhone = "07904258674", DeliveryInstructions = "Internal use only", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = null, AddressType = "WarehousePartner", SiteName = "Liverpool Secondary Storage", Line1 = "402 Logistics Way", Line2 = null, City = "Liverpool", County = "Merseyside", Postcode = "L3 1DD", Country = "United Kingdom", ContactName = "Ben Turner", ContactPhone = "07679196799", DeliveryInstructions = "Internal use only", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true },
                new Address { CustomerId = null, AddressType = "WarehousePartner", SiteName = "Warrington Hazardous Goods Depot", Line1 = "403 Logistics Way", Line2 = null, City = "Warrington", County = "Cheshire", Postcode = "WA1 2ZZ", Country = "United Kingdom", ContactName = "Rachel Morgan", ContactPhone = "07155926488", DeliveryInstructions = "Internal use only", IsPrimary = true, CreatedAt = DateTime.UtcNow, IsActive = true }
            };

            await _dbContext.Addresses.AddRangeAsync(addresses);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // X. Warehouses
        // =========================
        if (!await _dbContext.Warehouses.AnyAsync())
        {
            var warehouses = new List<Warehouse>
    {
        new Warehouse
        {
            Code = "RUN-01",
            Name = "Runcorn Central Warehouse",
            AddressId = 107,
            ContactName = "Nathan Price",
            ContactPhone = "07904258674",
            IsActive = true
        },
        new Warehouse
        {
            Code = "LIV-02",
            Name = "Liverpool Secondary Storage",
            AddressId = 108,
            ContactName = "Ben Turner",
            ContactPhone = "07679196799",
            IsActive = true
        },
        new Warehouse
        {
            Code = "WAR-03",
            Name = "Warrington Hazardous Goods Depot",
            AddressId = 109,
            ContactName = "Rachel Morgan",
            ContactPhone = "07155926488",
            IsActive = true
        }
    };

            await _dbContext.Warehouses.AddRangeAsync(warehouses);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // X. Projects
        // =========================
        if (!await _dbContext.Projects.AnyAsync())
        {
            var projectItems = new List<Project>
    {
            new Project
            {
                CustomerId = 2,
                ProjectCode = "PRJ-1001",
                ProjectName = "Boiler Water Treatment Refresh",
                Description = "12-month refresh programme for water treatment chemistry and consumables.",
                StartDate = new DateTime(2026, 1, 10),
                EndDate = new DateTime(2026, 12, 20),
                Status = "Active"
            },
            new Project
            {
                CustomerId = 10,
                ProjectCode = "PRJ-1002",
                ProjectName = "New Production Line Cleaning Rollout",
                Description = "Initial rollout of CIP and sanitation products for a new production line.",
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 9, 30),
                Status = "Active"
            },
            new Project
            {
                CustomerId = 9,
                ProjectCode = "PRJ-1003",
                ProjectName = "Lab Solvent Supply Contract Renewal",
                Description = "Contract renewal and rationalisation of laboratory solvent supply.",
                StartDate = new DateTime(2026, 1, 15),
                EndDate = new DateTime(2026, 8, 31),
                Status = "In Review"
            },
            new Project
            {
                CustomerId = 19,
                ProjectCode = "PRJ-1004",
                ProjectName = "Blend Plant Expansion Support",
                Description = "Supply support for pilot-scale blend plant capacity expansion.",
                StartDate = new DateTime(2025, 11, 5),
                EndDate = new DateTime(2026, 7, 31),
                Status = "Active"
            },
            new Project
            {
                CustomerId = 5,
                ProjectCode = "PRJ-1005",
                ProjectName = "Coatings Line Solvent Optimisation",
                Description = "Customer trial of alternative solvents and degreasing products.",
                StartDate = new DateTime(2025, 10, 20),
                EndDate = new DateTime(2026, 5, 31),
                Status = "Closing"
            },
            new Project
            {
                CustomerId = 20,
                ProjectCode = "PRJ-1006",
                ProjectName = "Municipal Dosing Chemical Framework",
                Description = "Framework agreement covering strategic water-treatment chemicals.",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2027, 1, 1),
                Status = "Active"
            }
        };

            await _dbContext.Projects.AddRangeAsync(projectItems);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // 4. Products
        // =========================
        if (!await _dbContext.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product { SKU = "ACET-25", ProductName = "Acetone 25L Drum", Description = "Acetone 25L Drum supplied for industrial and commercial use.", ProductCategoryId = 1, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 85.00m, Currency = "GBP", HazardClassId = 2, UNNumber = "UN1090", StorageRequirement = "Flammable store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 7) },
                new Product { SKU = "IPA-20", ProductName = "Isopropyl Alcohol 99.9% 20L", Description = "Isopropyl Alcohol 99.9% 20L supplied for industrial and commercial use.", ProductCategoryId = 1, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 92.50m, Currency = "GBP", HazardClassId = 2, UNNumber = "UN1219", StorageRequirement = "Flammable store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 13) },
                new Product { SKU = "METH-25", ProductName = "Methanol 25L Drum", Description = "Methanol 25L Drum supplied for industrial and commercial use.", ProductCategoryId = 1, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 88.00m, Currency = "GBP", HazardClassId = 4, UNNumber = "UN1230", StorageRequirement = "Flammable store", RequiresSds = true, IsRestricted = true, IsActive = true, CreatedAt = new DateTime(2024, 2, 19) },
                new Product { SKU = "ETHD-205", ProductName = "Ethanol Denatured 205L Drum", Description = "Ethanol Denatured 205L Drum supplied for industrial and commercial use.", ProductCategoryId = 1, UnitOfMeasureId = 3, PackSize = "205L", BasePrice = 610.00m, Currency = "GBP", HazardClassId = 2, UNNumber = "UN1170", StorageRequirement = "Flammable store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 25) },
                new Product { SKU = "WS-25", ProductName = "White Spirit 25L", Description = "White Spirit 25L supplied for industrial and commercial use.", ProductCategoryId = 1, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 74.00m, Currency = "GBP", HazardClassId = 2, UNNumber = "UN1300", StorageRequirement = "Flammable store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 3, 2) },
                new Product { SKU = "XYLE-20", ProductName = "Xylene Blend 20L", Description = "Xylene Blend 20L supplied for industrial and commercial use.", ProductCategoryId = 1, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 105.00m, Currency = "GBP", HazardClassId = 2, UNNumber = "UN1307", StorageRequirement = "Flammable store", RequiresSds = true, IsRestricted = true, IsActive = true, CreatedAt = new DateTime(2024, 3, 8) },
                new Product { SKU = "HCL-32-25", ProductName = "Hydrochloric Acid 32% 25L", Description = "Hydrochloric Acid 32% 25L supplied for industrial and commercial use.", ProductCategoryId = 2, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 69.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1789", StorageRequirement = "Acid bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 3, 14) },
                new Product { SKU = "H2SO4-96-25", ProductName = "Sulphuric Acid 96% 25L", Description = "Sulphuric Acid 96% 25L supplied for industrial and commercial use.", ProductCategoryId = 2, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 95.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1830", StorageRequirement = "Acid bay", RequiresSds = true, IsRestricted = true, IsActive = true, CreatedAt = new DateTime(2024, 3, 20) },
                new Product { SKU = "HNO3-10-10", ProductName = "Nitric Acid 10% 10L", Description = "Nitric Acid 10% 10L supplied for industrial and commercial use.", ProductCategoryId = 2, UnitOfMeasureId = 3, PackSize = "10L", BasePrice = 56.00m, Currency = "GBP", HazardClassId = 5, UNNumber = "UN2031", StorageRequirement = "Acid bay", RequiresSds = true, IsRestricted = true, IsActive = true, CreatedAt = new DateTime(2024, 3, 26) },
                new Product { SKU = "PHOS-85-25", ProductName = "Phosphoric Acid 85% 25L", Description = "Phosphoric Acid 85% 25L supplied for industrial and commercial use.", ProductCategoryId = 2, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 81.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1805", StorageRequirement = "Acid bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 4, 1) },
                new Product { SKU = "CIT-25", ProductName = "Citric Acid Solution 25L", Description = "Citric Acid Solution 25L supplied for industrial and commercial use.", ProductCategoryId = 2, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 41.50m, Currency = "GBP", HazardClassId = 6, UNNumber = "UN0000", StorageRequirement = "General chemical store", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 4, 7) },
                new Product { SKU = "NAOH-25", ProductName = "Sodium Hydroxide 25kg", Description = "Sodium Hydroxide 25kg supplied for industrial and commercial use.", ProductCategoryId = 3, UnitOfMeasureId = 7, PackSize = "25kg", BasePrice = 49.50m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1823", StorageRequirement = "Alkali bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 4, 13) },
                new Product { SKU = "KOH-25", ProductName = "Potassium Hydroxide 25kg", Description = "Potassium Hydroxide 25kg supplied for industrial and commercial use.", ProductCategoryId = 3, UnitOfMeasureId = 7, PackSize = "25kg", BasePrice = 62.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1813", StorageRequirement = "Alkali bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 4, 19) },
                new Product { SKU = "CAUS-25", ProductName = "Caustic Soda Solution 25L", Description = "Caustic Soda Solution 25L supplied for industrial and commercial use.", ProductCategoryId = 3, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 44.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1824", StorageRequirement = "Alkali bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 4, 25) },
                new Product { SKU = "HYPO-25", ProductName = "Sodium Hypochlorite 25L", Description = "Sodium Hypochlorite 25L supplied for industrial and commercial use.", ProductCategoryId = 4, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 38.00m, Currency = "GBP", HazardClassId = 7, UNNumber = "UN1791", StorageRequirement = "Cool chemical store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 5, 1) },
                new Product { SKU = "FERR-25", ProductName = "Ferric Chloride 25L", Description = "Ferric Chloride 25L supplied for industrial and commercial use.", ProductCategoryId = 4, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 52.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN2582", StorageRequirement = "Water treatment bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 5, 7) },
                new Product { SKU = "WT200-25", ProductName = "Coagulant Blend WT-200 25L", Description = "Coagulant Blend WT-200 25L supplied for industrial and commercial use.", ProductCategoryId = 4, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 57.50m, Currency = "GBP", HazardClassId = 6, UNNumber = "UN0000", StorageRequirement = "Water treatment bay", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 5, 13) },
                new Product { SKU = "PHBUF-5", ProductName = "pH Buffer Solution 5L", Description = "pH Buffer Solution 5L supplied for industrial and commercial use.", ProductCategoryId = 4, UnitOfMeasureId = 5, PackSize = "5L", BasePrice = 22.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Laboratory shelf", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 5, 19) },
                new Product { SKU = "AFOAM-10", ProductName = "Anti-Foam Agent 10L", Description = "Anti-Foam Agent 10L supplied for industrial and commercial use.", ProductCategoryId = 4, UnitOfMeasureId = 3, PackSize = "10L", BasePrice = 48.50m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "General chemical store", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 5, 25) },
                new Product { SKU = "DEGR-20", ProductName = "Industrial Degreaser 20L", Description = "Industrial Degreaser 20L supplied for industrial and commercial use.", ProductCategoryId = 5, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 46.00m, Currency = "GBP", HazardClassId = 6, UNNumber = "UN0000", StorageRequirement = "Cleaning bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 5, 31) },
                new Product { SKU = "FLOOR-20", ProductName = "Heavy Duty Floor Cleaner 20L", Description = "Heavy Duty Floor Cleaner 20L supplied for industrial and commercial use.", ProductCategoryId = 5, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 36.00m, Currency = "GBP", HazardClassId = 6, UNNumber = "UN0000", StorageRequirement = "Cleaning bay", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 6, 6) },
                new Product { SKU = "NSC-5", ProductName = "Neutral Surface Cleaner 5L", Description = "Neutral Surface Cleaner 5L supplied for industrial and commercial use.", ProductCategoryId = 5, UnitOfMeasureId = 5, PackSize = "5L", BasePrice = 15.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Cleaning bay", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 6, 12) },
                new Product { SKU = "SANI-10", ProductName = "Chlorinated Sanitiser 10L", Description = "Chlorinated Sanitiser 10L supplied for industrial and commercial use.", ProductCategoryId = 5, UnitOfMeasureId = 3, PackSize = "10L", BasePrice = 29.00m, Currency = "GBP", HazardClassId = 6, UNNumber = "UN0000", StorageRequirement = "Cleaning bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 6, 18) },
                new Product { SKU = "CIP-25", ProductName = "CIP Alkaline Cleaner 25L", Description = "CIP Alkaline Cleaner 25L supplied for industrial and commercial use.", ProductCategoryId = 5, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 51.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1824", StorageRequirement = "Cleaning bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 6, 24) },
                new Product { SKU = "FGDESC-20", ProductName = "Food Grade Descaler 20L", Description = "Food Grade Descaler 20L supplied for industrial and commercial use.", ProductCategoryId = 7, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 54.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN3265", StorageRequirement = "Food-safe chemical store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 6, 30) },
                new Product { SKU = "DIW-25", ProductName = "Deionised Water 25L", Description = "Deionised Water 25L supplied for industrial and commercial use.", ProductCategoryId = 6, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 18.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Laboratory shelf", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 7, 6) },
                new Product { SKU = "BUF4-1", ProductName = "Buffer Solution pH 4.0", Description = "Buffer Solution pH 4.0 supplied for industrial and commercial use.", ProductCategoryId = 6, UnitOfMeasureId = 5, PackSize = "1L", BasePrice = 12.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Laboratory shelf", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 7, 12) },
                new Product { SKU = "BUF7-1", ProductName = "Buffer Solution pH 7.0", Description = "Buffer Solution pH 7.0 supplied for industrial and commercial use.", ProductCategoryId = 6, UnitOfMeasureId = 5, PackSize = "1L", BasePrice = 12.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Laboratory shelf", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 7, 18) },
                new Product { SKU = "BUF10-1", ProductName = "Buffer Solution pH 10.0", Description = "Buffer Solution pH 10.0 supplied for industrial and commercial use.", ProductCategoryId = 6, UnitOfMeasureId = 5, PackSize = "1L", BasePrice = 12.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Laboratory shelf", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 7, 24) },
                new Product { SKU = "NACL-AR-5", ProductName = "Sodium Chloride AR Grade 5kg", Description = "Sodium Chloride AR Grade 5kg supplied for industrial and commercial use.", ProductCategoryId = 6, UnitOfMeasureId = 7, PackSize = "5kg", BasePrice = 27.50m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Laboratory shelf", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 7, 30) },
                new Product { SKU = "KNO3-5", ProductName = "Potassium Nitrate 5kg", Description = "Potassium Nitrate 5kg supplied for industrial and commercial use.", ProductCategoryId = 6, UnitOfMeasureId = 7, PackSize = "5kg", BasePrice = 31.00m, Currency = "GBP", HazardClassId = 5, UNNumber = "UN1486", StorageRequirement = "Laboratory shelf", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 8, 5) },
                new Product { SKU = "FGCC-20", ProductName = "Food Grade Caustic Cleaner 20L", Description = "Food Grade Caustic Cleaner 20L supplied for industrial and commercial use.", ProductCategoryId = 7, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 58.00m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN1824", StorageRequirement = "Food-safe chemical store", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 8, 11) },
                new Product { SKU = "GLYC-25", ProductName = "Glycerine USP 25L", Description = "Glycerine USP 25L supplied for industrial and commercial use.", ProductCategoryId = 7, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 72.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Food-safe chemical store", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 8, 17) },
                new Product { SKU = "PG-25", ProductName = "Propylene Glycol USP 25L", Description = "Propylene Glycol USP 25L supplied for industrial and commercial use.", ProductCategoryId = 7, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 79.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Food-safe chemical store", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 8, 23) },
                new Product { SKU = "SPILL-KIT", ProductName = "Chemical Spill Kit", Description = "Chemical Spill Kit supplied for industrial and commercial use.", ProductCategoryId = 8, UnitOfMeasureId = 4, PackSize = "1 kit", BasePrice = 64.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Warehouse consumables", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 8, 29) },
                new Product { SKU = "ABS-PADS", ProductName = "Absorbent Pads Pack", Description = "Absorbent Pads Pack supplied for industrial and commercial use.", ProductCategoryId = 8, UnitOfMeasureId = 4, PackSize = "100 pads", BasePrice = 29.50m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Warehouse consumables", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 9, 4) },
                new Product { SKU = "HDPE-25", ProductName = "HDPE 25L Drum", Description = "HDPE 25L Drum supplied for industrial and commercial use.", ProductCategoryId = 8, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 14.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Warehouse consumables", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 9, 10) },
                new Product { SKU = "TPLABEL-50", ProductName = "Tamper-Proof Labels Pack", Description = "Tamper-Proof Labels Pack supplied for industrial and commercial use.", ProductCategoryId = 8, UnitOfMeasureId = 4, PackSize = "50 labels", BasePrice = 11.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Warehouse consumables", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 9, 16) },
                new Product { SKU = "SAMPLE-12", ProductName = "Sample Bottle Pack", Description = "Sample Bottle Pack supplied for industrial and commercial use.", ProductCategoryId = 8, UnitOfMeasureId = 4, PackSize = "12 bottles", BasePrice = 16.50m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Warehouse consumables", RequiresSds = false, IsRestricted = false, IsActive = false, CreatedAt = new DateTime(2024, 9, 22) },
                new Product { SKU = "PUMP-STD", ProductName = "Chemical Transfer Pump", Description = "Chemical Transfer Pump supplied for industrial and commercial use.", ProductCategoryId = 8, UnitOfMeasureId = 4, PackSize = "1 unit", BasePrice = 78.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Warehouse consumables", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 9, 28) },
                new Product { SKU = "BIOCIDE-20", ProductName = "Closed System Biocide 20L", Description = "Closed System Biocide 20L supplied for industrial and commercial use.", ProductCategoryId = 9, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 67.00m, Currency = "GBP", HazardClassId = 7, UNNumber = "UN3082", StorageRequirement = "Water treatment bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 10, 4) },
                new Product { SKU = "INHIB-20", ProductName = "Corrosion Inhibitor 20L", Description = "Corrosion Inhibitor 20L supplied for industrial and commercial use.", ProductCategoryId = 9, UnitOfMeasureId = 3, PackSize = "20L", BasePrice = 73.00m, Currency = "GBP", HazardClassId = 6, UNNumber = "UN0000", StorageRequirement = "Water treatment bay", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 10, 10) },
                new Product { SKU = "DEFAM-5", ProductName = "Defoamer Concentrate 5L", Description = "Defoamer Concentrate 5L supplied for industrial and commercial use.", ProductCategoryId = 9, UnitOfMeasureId = 5, PackSize = "5L", BasePrice = 34.00m, Currency = "GBP", HazardClassId = 1, UNNumber = "UN0000", StorageRequirement = "Water treatment bay", RequiresSds = false, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 10, 16) },
                new Product { SKU = "WTPH-25", ProductName = "Wastewater pH Reducer 25L", Description = "Wastewater pH Reducer 25L supplied for industrial and commercial use.", ProductCategoryId = 9, UnitOfMeasureId = 3, PackSize = "25L", BasePrice = 43.50m, Currency = "GBP", HazardClassId = 3, UNNumber = "UN3265", StorageRequirement = "Water treatment bay", RequiresSds = true, IsRestricted = false, IsActive = true, CreatedAt = new DateTime(2024, 10, 22) }
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
                    CustomerId = 2,
                    ProductId = 24,
                    OverridePrice = 49.76m,
                    MinimumOrderQuantity = 1m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 2,
                    ProductId = 2,
                    OverridePrice = 90.18m,
                    MinimumOrderQuantity = 2m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 2,
                    ProductId = 15,
                    OverridePrice = 35.80m,
                    MinimumOrderQuantity = 2m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Annual tender pricing",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 2,
                    ProductId = 32,
                    OverridePrice = 53.79m,
                    MinimumOrderQuantity = 1m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Annual tender pricing",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                new CustomerProductPrice
                {
                    CustomerId = 9,
                    ProductId = 33,
                    OverridePrice = 64.14m,
                    MinimumOrderQuantity = 1m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Annual tender pricing",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 9,
                    ProductId = 20,
                    OverridePrice = 43.46m,
                    MinimumOrderQuantity = 1m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 9,
                    ProductId = 24,
                    OverridePrice = 47.91m,
                    MinimumOrderQuantity = 5m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Strategic account rate",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 9,
                    ProductId = 15,
                    OverridePrice = 35.77m,
                    MinimumOrderQuantity = 2m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                new CustomerProductPrice
                {
                    CustomerId = 10,
                    ProductId = 44,
                    OverridePrice = 41.64m,
                    MinimumOrderQuantity = 1m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 10,
                    ProductId = 2,
                    OverridePrice = 84.61m,
                    MinimumOrderQuantity = 5m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Trial pricing agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 10,
                    ProductId = 15,
                    OverridePrice = 33.70m,
                    MinimumOrderQuantity = 4m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 10,
                    ProductId = 24,
                    OverridePrice = 49.21m,
                    MinimumOrderQuantity = 5m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Framework agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                new CustomerProductPrice
                {
                    CustomerId = 19,
                    ProductId = 32,
                    OverridePrice = 47.73m,
                    MinimumOrderQuantity = 2m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Trial pricing agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 19,
                    ProductId = 24,
                    OverridePrice = 45.51m,
                    MinimumOrderQuantity = 5m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Strategic account rate",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 19,
                    ProductId = 33,
                    OverridePrice = 62.30m,
                    MinimumOrderQuantity = 4m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Trial pricing agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 19,
                    ProductId = 40,
                    OverridePrice = 72.37m,
                    MinimumOrderQuantity = 1m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Trial pricing agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                new CustomerProductPrice
                {
                    CustomerId = 20,
                    ProductId = 33,
                    OverridePrice = 64.19m,
                    MinimumOrderQuantity = 5m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Trial pricing agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 20,
                    ProductId = 2,
                    OverridePrice = 74.40m,
                    MinimumOrderQuantity = 4m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Annual tender pricing",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 20,
                    ProductId = 44,
                    OverridePrice = 37.98m,
                    MinimumOrderQuantity = 4m,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    Notes = "Trial pricing agreement",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new CustomerProductPrice
                {
                    CustomerId = 20,
                    ProductId = 20,
                    OverridePrice = 39.15m,
                    MinimumOrderQuantity = 4m,
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
        // X. Orders
        // =========================
        if (!await _dbContext.Orders.AnyAsync())
        {
            var orders = new List<Order>
            {
                new Order
                {
                    OrderNumber = "ORD-2026-001001",
                    CustomerId = 17,
                    ProjectId = null,
                    DeliveryAddressId = 72,
                    BillingAddressId = 69,
                    CreatedByUserId = 4,
                    AssignedToUserId = null,
                    WarehouseId = 2,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 21),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 16, 14, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 4, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1601.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 320.20m,
                    TotalAmount = 1921.20m,
                    PurchaseOrderReference = "PO-017-5001",
                    SpecialInstructions = "Use customer pallet labels.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001002",
                    CustomerId = 8,
                    ProjectId = null,
                    DeliveryAddressId = 33,
                    BillingAddressId = 30,
                    CreatedByUserId = 3,
                    AssignedToUserId = null,
                    WarehouseId = 2,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 23),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 18, 7, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 19, 5, 30, 0),
                    Currency = "GBP",
                    Subtotal = 839.50m,
                    DiscountAmount = 25.18m,
                    TaxAmount = 162.86m,
                    TotalAmount = 977.18m,
                    PurchaseOrderReference = "PO-008-5001",
                    SpecialInstructions = "Do not mix with food-safe product loads.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001003",
                    CustomerId = 2,
                    ProjectId = null,
                    DeliveryAddressId = 7,
                    BillingAddressId = 5,
                    CreatedByUserId = 3,
                    AssignedToUserId = null,
                    WarehouseId = 3,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 23),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 17, 20, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 23, 17, 30, 0),
                    Currency = "GBP",
                    Subtotal = 2166.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 433.20m,
                    TotalAmount = 2599.20m,
                    PurchaseOrderReference = "PO-002-5001",
                    SpecialInstructions = "Deliver with updated SDS bundle.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001004",
                    CustomerId = 19,
                    ProjectId = null,
                    DeliveryAddressId = 79,
                    BillingAddressId = 77,
                    CreatedByUserId = 5,
                    AssignedToUserId = null,
                    WarehouseId = 1,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 21),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 16, 14, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 21, 20, 30, 0),
                    Currency = "GBP",
                    Subtotal = 2206.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 441.20m,
                    TotalAmount = 2647.20m,
                    PurchaseOrderReference = "PO-019-5001",
                    SpecialInstructions = "Use customer pallet labels.",
                    InternalNotes = "Priority strategic account.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001005",
                    CustomerId = 8,
                    ProjectId = null,
                    DeliveryAddressId = 32,
                    BillingAddressId = 30,
                    CreatedByUserId = 4,
                    AssignedToUserId = null,
                    WarehouseId = 3,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 20),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 17, 20, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 17, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1450.50m,
                    DiscountAmount = 43.52m,
                    TaxAmount = 281.40m,
                    TotalAmount = 1688.38m,
                    PurchaseOrderReference = "PO-008-5001",
                    SpecialInstructions = "Notify buyer on dispatch.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001006",
                    CustomerId = 18,
                    ProjectId = null,
                    DeliveryAddressId = 76,
                    BillingAddressId = 73,
                    CreatedByUserId = 4,
                    AssignedToUserId = null,
                    WarehouseId = 2,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 22),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 18, 2, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 18, 16, 30, 0),
                    Currency = "GBP",
                    Subtotal = 207.50m,
                    DiscountAmount = 6.22m,
                    TaxAmount = 40.26m,
                    TotalAmount = 241.54m,
                    PurchaseOrderReference = "PO-018-5001",
                    SpecialInstructions = null,
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001007",
                    CustomerId = 13,
                    ProjectId = null,
                    DeliveryAddressId = 55,
                    BillingAddressId = 52,
                    CreatedByUserId = 5,
                    AssignedToUserId = null,
                    WarehouseId = 1,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 27),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 17, 17, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 6, 30, 0),
                    Currency = "GBP",
                    Subtotal = 894.00m,
                    DiscountAmount = 26.82m,
                    TaxAmount = 173.44m,
                    TotalAmount = 1040.62m,
                    PurchaseOrderReference = "PO-013-5001",
                    SpecialInstructions = null,
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001008",
                    CustomerId = 16,
                    ProjectId = null,
                    DeliveryAddressId = 67,
                    BillingAddressId = 64,
                    CreatedByUserId = 5,
                    AssignedToUserId = null,
                    WarehouseId = 3,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 24),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 17, 11, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 24, 2, 30, 0),
                    Currency = "GBP",
                    Subtotal = 777.50m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 155.50m,
                    TotalAmount = 933.00m,
                    PurchaseOrderReference = "PO-016-5001",
                    SpecialInstructions = null,
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001009",
                    CustomerId = 2,
                    ProjectId = 1,
                    DeliveryAddressId = 7,
                    BillingAddressId = 5,
                    CreatedByUserId = 4,
                    AssignedToUserId = null,
                    WarehouseId = 2,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 27),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 17, 2, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 17, 22, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1591.96m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 318.39m,
                    TotalAmount = 1910.35m,
                    PurchaseOrderReference = "PO-002-5001",
                    SpecialInstructions = "Notify buyer on dispatch.",
                    InternalNotes = "Customer usually requests split delivery.",
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001010",
                    CustomerId = 10,
                    ProjectId = null,
                    DeliveryAddressId = 42,
                    BillingAddressId = 39,
                    CreatedByUserId = 1,
                    AssignedToUserId = null,
                    WarehouseId = 2,
                    CarrierId = null,
                    OrderStatusId = 1,
                    RequestedDeliveryDate = new DateTime(2025, 12, 24),
                    SubmittedAt = null,
                    CreatedAt = new DateTime(2025, 12, 18, 6, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 20, 23, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1125.50m,
                    DiscountAmount = 135.06m,
                    TaxAmount = 198.09m,
                    TotalAmount = 1188.53m,
                    PurchaseOrderReference = "PO-010-5001",
                    SpecialInstructions = "Do not mix with food-safe product loads.",
                    InternalNotes = "Customer usually requests split delivery.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001011",
                    CustomerId = 11,
                    ProjectId = null,
                    DeliveryAddressId = 45,
                    BillingAddressId = 43,
                    CreatedByUserId = 4,
                    AssignedToUserId = 8,
                    WarehouseId = 1,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 25),
                    SubmittedAt = new DateTime(2025, 12, 17, 23, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 21, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 18, 13, 30, 0),
                    Currency = "GBP",
                    Subtotal = 894.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 178.80m,
                    TotalAmount = 1072.80m,
                    PurchaseOrderReference = "PO-011-5001",
                    SpecialInstructions = "Schedule AM delivery if possible.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001012",
                    CustomerId = 22,
                    ProjectId = null,
                    DeliveryAddressId = 93,
                    BillingAddressId = 90,
                    CreatedByUserId = 4,
                    AssignedToUserId = 7,
                    WarehouseId = 1,
                    CarrierId = 3,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 23),
                    SubmittedAt = new DateTime(2025, 12, 17, 17, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 15, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 18, 4, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1328.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 265.60m,
                    TotalAmount = 1593.60m,
                    PurchaseOrderReference = "PO-022-5001",
                    SpecialInstructions = "Deliver with updated SDS bundle.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001013",
                    CustomerId = 10,
                    ProjectId = 2,
                    DeliveryAddressId = 42,
                    BillingAddressId = 39,
                    CreatedByUserId = 3,
                    AssignedToUserId = 7,
                    WarehouseId = 2,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 19),
                    SubmittedAt = new DateTime(2025, 12, 17, 7, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 5, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 5, 30, 0),
                    Currency = "GBP",
                    Subtotal = 421.00m,
                    DiscountAmount = 50.52m,
                    TaxAmount = 74.10m,
                    TotalAmount = 444.58m,
                    PurchaseOrderReference = "PO-010-5001",
                    SpecialInstructions = "Schedule AM delivery if possible.",
                    InternalNotes = "Restricted item requires approval.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001014",
                    CustomerId = 3,
                    ProjectId = null,
                    DeliveryAddressId = 12,
                    BillingAddressId = 9,
                    CreatedByUserId = 6,
                    AssignedToUserId = 9,
                    WarehouseId = 3,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 23),
                    SubmittedAt = new DateTime(2025, 12, 18, 0, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 22, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 18, 19, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1232.00m,
                    DiscountAmount = 36.96m,
                    TaxAmount = 239.01m,
                    TotalAmount = 1434.05m,
                    PurchaseOrderReference = "PO-003-5001",
                    SpecialInstructions = "Schedule AM delivery if possible.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001015",
                    CustomerId = 23,
                    ProjectId = null,
                    DeliveryAddressId = 97,
                    BillingAddressId = 94,
                    CreatedByUserId = 2,
                    AssignedToUserId = 7,
                    WarehouseId = 3,
                    CarrierId = 3,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 26),
                    SubmittedAt = new DateTime(2025, 12, 18, 1, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 23, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 18, 8, 30, 0),
                    Currency = "GBP",
                    Subtotal = 230.00m,
                    DiscountAmount = 6.90m,
                    TaxAmount = 44.62m,
                    TotalAmount = 267.72m,
                    PurchaseOrderReference = "PO-023-5001",
                    SpecialInstructions = "Schedule AM delivery if possible.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001016",
                    CustomerId = 5,
                    ProjectId = null,
                    DeliveryAddressId = 20,
                    BillingAddressId = 18,
                    CreatedByUserId = 3,
                    AssignedToUserId = 9,
                    WarehouseId = 2,
                    CarrierId = 3,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 25),
                    SubmittedAt = new DateTime(2025, 12, 17, 7, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 5, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 23, 15, 30, 0),
                    Currency = "GBP",
                    Subtotal = 650.00m,
                    DiscountAmount = 78.00m,
                    TaxAmount = 114.40m,
                    TotalAmount = 686.40m,
                    PurchaseOrderReference = "PO-005-5001",
                    SpecialInstructions = "Deliver with updated SDS bundle.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001017",
                    CustomerId = 8,
                    ProjectId = null,
                    DeliveryAddressId = 32,
                    BillingAddressId = 30,
                    CreatedByUserId = 2,
                    AssignedToUserId = 10,
                    WarehouseId = 3,
                    CarrierId = 3,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 24),
                    SubmittedAt = new DateTime(2025, 12, 17, 22, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 20, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 23, 4, 30, 0),
                    Currency = "GBP",
                    Subtotal = 384.00m,
                    DiscountAmount = 11.52m,
                    TaxAmount = 74.50m,
                    TotalAmount = 446.98m,
                    PurchaseOrderReference = "PO-008-5001",
                    SpecialInstructions = "Do not mix with food-safe product loads.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001018",
                    CustomerId = 4,
                    ProjectId = null,
                    DeliveryAddressId = 17,
                    BillingAddressId = 13,
                    CreatedByUserId = 2,
                    AssignedToUserId = 8,
                    WarehouseId = 1,
                    CarrierId = 1,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 19),
                    SubmittedAt = new DateTime(2025, 12, 17, 17, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 15, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 19, 8, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1588.00m,
                    DiscountAmount = 119.10m,
                    TaxAmount = 293.78m,
                    TotalAmount = 1762.68m,
                    PurchaseOrderReference = "PO-004-5001",
                    SpecialInstructions = "Notify buyer on dispatch.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001019",
                    CustomerId = 25,
                    ProjectId = null,
                    DeliveryAddressId = 106,
                    BillingAddressId = 103,
                    CreatedByUserId = 2,
                    AssignedToUserId = 11,
                    WarehouseId = 2,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 21),
                    SubmittedAt = new DateTime(2025, 12, 17, 13, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 11, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 18, 14, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1334.00m,
                    DiscountAmount = 160.08m,
                    TaxAmount = 234.78m,
                    TotalAmount = 1408.70m,
                    PurchaseOrderReference = "PO-025-5001",
                    SpecialInstructions = "Schedule AM delivery if possible.",
                    InternalNotes = "Priority strategic account.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001020",
                    CustomerId = 22,
                    ProjectId = null,
                    DeliveryAddressId = 92,
                    BillingAddressId = 90,
                    CreatedByUserId = 1,
                    AssignedToUserId = 7,
                    WarehouseId = 2,
                    CarrierId = 1,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 23),
                    SubmittedAt = new DateTime(2025, 12, 16, 14, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 16, 12, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 20, 22, 30, 0),
                    Currency = "GBP",
                    Subtotal = 610.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 122.00m,
                    TotalAmount = 732.00m,
                    PurchaseOrderReference = "PO-022-5001",
                    SpecialInstructions = "Deliver with updated SDS bundle.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001021",
                    CustomerId = 25,
                    ProjectId = null,
                    DeliveryAddressId = 106,
                    BillingAddressId = 103,
                    CreatedByUserId = 6,
                    AssignedToUserId = 11,
                    WarehouseId = 2,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 19),
                    SubmittedAt = new DateTime(2025, 12, 16, 18, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 16, 16, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 20, 13, 30, 0),
                    Currency = "GBP",
                    Subtotal = 588.50m,
                    DiscountAmount = 70.62m,
                    TaxAmount = 103.58m,
                    TotalAmount = 621.46m,
                    PurchaseOrderReference = "PO-025-5001",
                    SpecialInstructions = "Deliver with updated SDS bundle.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001022",
                    CustomerId = 3,
                    ProjectId = null,
                    DeliveryAddressId = 11,
                    BillingAddressId = 9,
                    CreatedByUserId = 5,
                    AssignedToUserId = 11,
                    WarehouseId = 2,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 19),
                    SubmittedAt = new DateTime(2025, 12, 16, 20, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 16, 18, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 20, 7, 30, 0),
                    Currency = "GBP",
                    Subtotal = 621.00m,
                    DiscountAmount = 18.63m,
                    TaxAmount = 120.47m,
                    TotalAmount = 722.84m,
                    PurchaseOrderReference = "PO-003-5001",
                    SpecialInstructions = null,
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001023",
                    CustomerId = 23,
                    ProjectId = null,
                    DeliveryAddressId = 96,
                    BillingAddressId = 94,
                    CreatedByUserId = 5,
                    AssignedToUserId = 9,
                    WarehouseId = 3,
                    CarrierId = 3,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 25),
                    SubmittedAt = new DateTime(2025, 12, 16, 20, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 16, 18, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 21, 4, 30, 0),
                    Currency = "GBP",
                    Subtotal = 186.00m,
                    DiscountAmount = 5.58m,
                    TaxAmount = 36.08m,
                    TotalAmount = 216.50m,
                    PurchaseOrderReference = "PO-023-5001",
                    SpecialInstructions = null,
                    InternalNotes = "Priority strategic account.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001024",
                    CustomerId = 23,
                    ProjectId = null,
                    DeliveryAddressId = 97,
                    BillingAddressId = 94,
                    CreatedByUserId = 6,
                    AssignedToUserId = 7,
                    WarehouseId = 1,
                    CarrierId = 2,
                    OrderStatusId = 2,
                    RequestedDeliveryDate = new DateTime(2025, 12, 23),
                    SubmittedAt = new DateTime(2025, 12, 17, 5, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 3, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 12, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1503.00m,
                    DiscountAmount = 45.09m,
                    TaxAmount = 291.58m,
                    TotalAmount = 1749.49m,
                    PurchaseOrderReference = "PO-023-5001",
                    SpecialInstructions = "Use customer pallet labels.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001025",
                    CustomerId = 5,
                    ProjectId = 5,
                    DeliveryAddressId = 20,
                    BillingAddressId = 18,
                    CreatedByUserId = 1,
                    AssignedToUserId = 10,
                    WarehouseId = 2,
                    CarrierId = 3,
                    OrderStatusId = 3,
                    RequestedDeliveryDate = new DateTime(2025, 12, 20),
                    SubmittedAt = new DateTime(2025, 12, 16, 15, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 16, 13, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 19, 7, 30, 0),
                    Currency = "GBP",
                    Subtotal = 440.00m,
                    DiscountAmount = 52.80m,
                    TaxAmount = 77.44m,
                    TotalAmount = 464.64m,
                    PurchaseOrderReference = "PO-005-5001",
                    SpecialInstructions = "Do not mix with food-safe product loads.",
                    InternalNotes = "Priority strategic account.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001026",
                    CustomerId = 2,
                    ProjectId = 1,
                    DeliveryAddressId = 8,
                    BillingAddressId = 5,
                    CreatedByUserId = 2,
                    AssignedToUserId = 7,
                    WarehouseId = 3,
                    CarrierId = 1,
                    OrderStatusId = 3,
                    RequestedDeliveryDate = new DateTime(2025, 12, 26),
                    SubmittedAt = new DateTime(2025, 12, 17, 4, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 2, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 17, 8, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1040.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 208.00m,
                    TotalAmount = 1248.00m,
                    PurchaseOrderReference = "PO-002-5001",
                    SpecialInstructions = "Use customer pallet labels.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001027",
                    CustomerId = 11,
                    ProjectId = null,
                    DeliveryAddressId = 45,
                    BillingAddressId = 43,
                    CreatedByUserId = 3,
                    AssignedToUserId = 7,
                    WarehouseId = 3,
                    CarrierId = 3,
                    OrderStatusId = 3,
                    RequestedDeliveryDate = new DateTime(2025, 12, 25),
                    SubmittedAt = new DateTime(2025, 12, 17, 18, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 16, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 21, 1, 30, 0),
                    Currency = "GBP",
                    Subtotal = 348.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 69.60m,
                    TotalAmount = 417.60m,
                    PurchaseOrderReference = "PO-011-5001",
                    SpecialInstructions = "Deliver with updated SDS bundle.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001028",
                    CustomerId = 15,
                    ProjectId = null,
                    DeliveryAddressId = 62,
                    BillingAddressId = 60,
                    CreatedByUserId = 2,
                    AssignedToUserId = 9,
                    WarehouseId = 2,
                    CarrierId = 1,
                    OrderStatusId = 3,
                    RequestedDeliveryDate = new DateTime(2025, 12, 24),
                    SubmittedAt = new DateTime(2025, 12, 16, 16, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 16, 14, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 17, 6, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1630.00m,
                    DiscountAmount = 195.60m,
                    TaxAmount = 286.88m,
                    TotalAmount = 1721.28m,
                    PurchaseOrderReference = "PO-015-5001",
                    SpecialInstructions = null,
                    InternalNotes = "Priority strategic account.",
                    FailureReason = null,
                    IsPriorityOrder = true
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001029",
                    CustomerId = 12,
                    ProjectId = null,
                    DeliveryAddressId = 49,
                    BillingAddressId = 47,
                    CreatedByUserId = 2,
                    AssignedToUserId = 8,
                    WarehouseId = 3,
                    CarrierId = 3,
                    OrderStatusId = 3,
                    RequestedDeliveryDate = new DateTime(2025, 12, 20),
                    SubmittedAt = new DateTime(2025, 12, 17, 2, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 17, 0, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 20, 30, 0),
                    Currency = "GBP",
                    Subtotal = 366.00m,
                    DiscountAmount = 0.00m,
                    TaxAmount = 73.20m,
                    TotalAmount = 439.20m,
                    PurchaseOrderReference = "PO-012-5001",
                    SpecialInstructions = "Do not mix with food-safe product loads.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                },
                new Order
                {
                    OrderNumber = "ORD-2026-001030",
                    CustomerId = 3,
                    ProjectId = null,
                    DeliveryAddressId = 12,
                    BillingAddressId = 9,
                    CreatedByUserId = 6,
                    AssignedToUserId = 10,
                    WarehouseId = 3,
                    CarrierId = 1,
                    OrderStatusId = 3,
                    RequestedDeliveryDate = new DateTime(2025, 12, 24),
                    SubmittedAt = new DateTime(2025, 12, 18, 3, 30, 0),
                    CreatedAt = new DateTime(2025, 12, 18, 1, 30, 0),
                    UpdatedAt = new DateTime(2025, 12, 22, 20, 30, 0),
                    Currency = "GBP",
                    Subtotal = 1071.50m,
                    DiscountAmount = 32.14m,
                    TaxAmount = 207.87m,
                    TotalAmount = 1247.23m,
                    PurchaseOrderReference = "PO-003-5001",
                    SpecialInstructions = "Do not mix with food-safe product loads.",
                    InternalNotes = null,
                    FailureReason = null,
                    IsPriorityOrder = false
                }
            };

            await _dbContext.Orders.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // XI. Order Items
        // =========================
        if (!await _dbContext.OrderItems.AnyAsync())
        {
            var orderLookup = await _dbContext.Orders
                .Where(o => o.OrderNumber.CompareTo("ORD-2026-001001") >= 0 &&
                            o.OrderNumber.CompareTo("ORD-2026-001030") <= 0)
                .ToDictionaryAsync(o => o.OrderNumber, o => o.OrderId);

            var orderItems = new List<OrderItem>
            {
                // ORD-2026-001001
                new OrderItem { OrderId = orderLookup["ORD-2026-001001"], ProductId = 31, Quantity = 8, UnitPrice = 31.00m, DiscountPercent = 0.0m, LineTotal = 248.00m, Notes = "Check stock before confirmation" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001001"], ProductId = 41, Quantity = 12, UnitPrice = 67.00m, DiscountPercent = 0.0m, LineTotal = 804.00m, Notes = "ADR packaging required" },

                // ORD-2026-001002
                new OrderItem { OrderId = orderLookup["ORD-2026-001002"], ProductId = 12, Quantity = 8, UnitPrice = 49.50m, DiscountPercent = 3.0m, LineTotal = 384.12m, Notes = "Customer requested standard packaging" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001002"], ProductId = 27, Quantity = 10, UnitPrice = 12.00m, DiscountPercent = 0.0m, LineTotal = 120.00m, Notes = null },

                // ORD-2026-001003
                new OrderItem { OrderId = orderLookup["ORD-2026-001003"], ProductId = 2, Quantity = 10, UnitPrice = 92.50m, DiscountPercent = 0.0m, LineTotal = 925.00m, Notes = "Check stock before confirmation" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001003"], ProductId = 13, Quantity = 12, UnitPrice = 62.00m, DiscountPercent = 0.0m, LineTotal = 744.00m, Notes = "Customer requested standard packaging" },

                // ORD-2026-001004
                new OrderItem { OrderId = orderLookup["ORD-2026-001004"], ProductId = 4, Quantity = 3, UnitPrice = 610.00m, DiscountPercent = 12.0m, LineTotal = 1610.40m, Notes = "ADR packaging required" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001004"], ProductId = 22, Quantity = 4, UnitPrice = 15.00m, DiscountPercent = 0.0m, LineTotal = 60.00m, Notes = null },

                // ORD-2026-001005
                new OrderItem { OrderId = orderLookup["ORD-2026-001005"], ProductId = 8, Quantity = 8, UnitPrice = 95.00m, DiscountPercent = 3.0m, LineTotal = 737.20m, Notes = "Customer requested standard packaging" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001005"], ProductId = 24, Quantity = 10, UnitPrice = 51.00m, DiscountPercent = 3.0m, LineTotal = 494.70m, Notes = "Notify buyer on dispatch" },

                // ORD-2026-001006
                new OrderItem { OrderId = orderLookup["ORD-2026-001006"], ProductId = 18, Quantity = 4, UnitPrice = 22.00m, DiscountPercent = 3.0m, LineTotal = 85.36m, Notes = null },
                new OrderItem { OrderId = orderLookup["ORD-2026-001006"], ProductId = 37, Quantity = 6, UnitPrice = 14.00m, DiscountPercent = 0.0m, LineTotal = 84.00m, Notes = "Check stock before confirmation" },

                // ORD-2026-001007
                new OrderItem { OrderId = orderLookup["ORD-2026-001007"], ProductId = 13, Quantity = 8, UnitPrice = 62.00m, DiscountPercent = 3.0m, LineTotal = 481.12m, Notes = null },
                new OrderItem { OrderId = orderLookup["ORD-2026-001007"], ProductId = 20, Quantity = 8, UnitPrice = 46.00m, DiscountPercent = 3.0m, LineTotal = 356.96m, Notes = null },

                // ORD-2026-001008
                new OrderItem { OrderId = orderLookup["ORD-2026-001008"], ProductId = 36, Quantity = 10, UnitPrice = 29.50m, DiscountPercent = 0.0m, LineTotal = 295.00m, Notes = "Check stock before confirmation" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001008"], ProductId = 17, Quantity = 8, UnitPrice = 57.50m, DiscountPercent = 0.0m, LineTotal = 460.00m, Notes = null },

                // ORD-2026-001009
                new OrderItem { OrderId = orderLookup["ORD-2026-001009"], ProductId = 4, Quantity = 2, UnitPrice = 610.00m, DiscountPercent = 12.0m, LineTotal = 1073.60m, Notes = "Customer usually requests split delivery" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001009"], ProductId = 20, Quantity = 8, UnitPrice = 46.00m, DiscountPercent = 0.0m, LineTotal = 368.00m, Notes = null },

                // ORD-2026-001010
                new OrderItem { OrderId = orderLookup["ORD-2026-001010"], ProductId = 10, Quantity = 10, UnitPrice = 81.00m, DiscountPercent = 12.0m, LineTotal = 712.80m, Notes = "Do not mix with food-safe product loads." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001010"], ProductId = 13, Quantity = 5, UnitPrice = 62.00m, DiscountPercent = 12.0m, LineTotal = 272.80m, Notes = null },

                // ORD-2026-001011
                new OrderItem { OrderId = orderLookup["ORD-2026-001011"], ProductId = 15, Quantity = 10, UnitPrice = 38.00m, DiscountPercent = 0.0m, LineTotal = 380.00m, Notes = "Schedule AM delivery if possible." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001011"], ProductId = 35, Quantity = 8, UnitPrice = 64.00m, DiscountPercent = 0.0m, LineTotal = 512.00m, Notes = null },

                // ORD-2026-001012
                new OrderItem { OrderId = orderLookup["ORD-2026-001012"], ProductId = 25, Quantity = 12, UnitPrice = 54.00m, DiscountPercent = 0.0m, LineTotal = 648.00m, Notes = "Deliver with updated SDS bundle." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001012"], ProductId = 40, Quantity = 8, UnitPrice = 78.00m, DiscountPercent = 0.0m, LineTotal = 624.00m, Notes = null },

                // ORD-2026-001013
                new OrderItem { OrderId = orderLookup["ORD-2026-001013"], ProductId = 10, Quantity = 2, UnitPrice = 81.00m, DiscountPercent = 12.0m, LineTotal = 142.56m, Notes = "Restricted item requires approval." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001013"], ProductId = 23, Quantity = 8, UnitPrice = 29.00m, DiscountPercent = 0.0m, LineTotal = 232.00m, Notes = null },

                // ORD-2026-001014
                new OrderItem { OrderId = orderLookup["ORD-2026-001014"], ProductId = 3, Quantity = 10, UnitPrice = 88.00m, DiscountPercent = 3.0m, LineTotal = 853.60m, Notes = "Schedule AM delivery if possible." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001014"], ProductId = 35, Quantity = 6, UnitPrice = 64.00m, DiscountPercent = 0.0m, LineTotal = 384.00m, Notes = null },

                // ORD-2026-001015
                new OrderItem { OrderId = orderLookup["ORD-2026-001015"], ProductId = 21, Quantity = 2, UnitPrice = 36.00m, DiscountPercent = 0.0m, LineTotal = 72.00m, Notes = "Schedule AM delivery if possible." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001015"], ProductId = 37, Quantity = 10, UnitPrice = 14.00m, DiscountPercent = 0.0m, LineTotal = 140.00m, Notes = null },

                // ORD-2026-001016
                new OrderItem { OrderId = orderLookup["ORD-2026-001016"], ProductId = 24, Quantity = 8, UnitPrice = 51.00m, DiscountPercent = 12.0m, LineTotal = 359.04m, Notes = "Deliver with updated SDS bundle." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001016"], ProductId = 30, Quantity = 8, UnitPrice = 27.50m, DiscountPercent = 12.0m, LineTotal = 193.60m, Notes = null },

                // ORD-2026-001017
                new OrderItem { OrderId = orderLookup["ORD-2026-001017"], ProductId = 8, Quantity = 3, UnitPrice = 95.00m, DiscountPercent = 12.0m, LineTotal = 250.80m, Notes = "Customer requested standard packaging" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001017"], ProductId = 18, Quantity = 4, UnitPrice = 22.00m, DiscountPercent = 12.0m, LineTotal = 77.44m, Notes = null },

                // ORD-2026-001018
                new OrderItem { OrderId = orderLookup["ORD-2026-001018"], ProductId = 2, Quantity = 10, UnitPrice = 90.18m, DiscountPercent = 0.0m, LineTotal = 901.80m, Notes = "Notify buyer on dispatch." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001018"], ProductId = 4, Quantity = 1, UnitPrice = 610.00m, DiscountPercent = 12.0m, LineTotal = 536.80m, Notes = null },

                // ORD-2026-001019
                new OrderItem { OrderId = orderLookup["ORD-2026-001019"], ProductId = 20, Quantity = 12, UnitPrice = 46.00m, DiscountPercent = 0.0m, LineTotal = 552.00m, Notes = "Priority strategic account." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001019"], ProductId = 42, Quantity = 8, UnitPrice = 73.00m, DiscountPercent = 12.0m, LineTotal = 513.92m, Notes = null },

                // ORD-2026-001020
                new OrderItem { OrderId = orderLookup["ORD-2026-001020"], ProductId = 22, Quantity = 10, UnitPrice = 15.00m, DiscountPercent = 0.0m, LineTotal = 150.00m, Notes = "Deliver with updated SDS bundle." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001020"], ProductId = 35, Quantity = 6, UnitPrice = 64.00m, DiscountPercent = 0.0m, LineTotal = 384.00m, Notes = null },

                // ORD-2026-001021
                new OrderItem { OrderId = orderLookup["ORD-2026-001021"], ProductId = 13, Quantity = 3, UnitPrice = 62.00m, DiscountPercent = 12.0m, LineTotal = 163.68m, Notes = "ADR packaging required" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001021"], ProductId = 21, Quantity = 10, UnitPrice = 36.00m, DiscountPercent = 0.0m, LineTotal = 360.00m, Notes = "Customer requested standard packaging" },

                // ORD-2026-001022
                new OrderItem { OrderId = orderLookup["ORD-2026-001022"], ProductId = 31, Quantity = 12, UnitPrice = 31.00m, DiscountPercent = 0.0m, LineTotal = 372.00m, Notes = null },
                new OrderItem { OrderId = orderLookup["ORD-2026-001022"], ProductId = 25, Quantity = 3, UnitPrice = 54.00m, DiscountPercent = 0.0m, LineTotal = 162.00m, Notes = "Check stock before confirmation" },

                // ORD-2026-001023
                new OrderItem { OrderId = orderLookup["ORD-2026-001023"], ProductId = 6, Quantity = 1, UnitPrice = 105.00m, DiscountPercent = 0.0m, LineTotal = 105.00m, Notes = "Priority strategic account." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001023"], ProductId = 37, Quantity = 3, UnitPrice = 14.00m, DiscountPercent = 0.0m, LineTotal = 42.00m, Notes = null },

                // ORD-2026-001024
                new OrderItem { OrderId = orderLookup["ORD-2026-001024"], ProductId = 4, Quantity = 2, UnitPrice = 610.00m, DiscountPercent = 12.0m, LineTotal = 1073.60m, Notes = "Use customer pallet labels." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001024"], ProductId = 15, Quantity = 10, UnitPrice = 38.00m, DiscountPercent = 0.0m, LineTotal = 380.00m, Notes = null },

                // ORD-2026-001025
                new OrderItem { OrderId = orderLookup["ORD-2026-001025"], ProductId = 35, Quantity = 1, UnitPrice = 64.00m, DiscountPercent = 12.0m, LineTotal = 56.32m, Notes = "Priority strategic account." },
                new OrderItem { OrderId = orderLookup["ORD-2026-001025"], ProductId = 28, Quantity = 6, UnitPrice = 12.00m, DiscountPercent = 12.0m, LineTotal = 63.36m, Notes = null },

                // ORD-2026-001026
                new OrderItem { OrderId = orderLookup["ORD-2026-001026"], ProductId = 20, Quantity = 12, UnitPrice = 46.00m, DiscountPercent = 3.0m, LineTotal = 535.44m, Notes = null },
                new OrderItem { OrderId = orderLookup["ORD-2026-001026"], ProductId = 13, Quantity = 4, UnitPrice = 62.00m, DiscountPercent = 3.0m, LineTotal = 240.56m, Notes = "ADR packaging required" },

                // ORD-2026-001027
                new OrderItem { OrderId = orderLookup["ORD-2026-001027"], ProductId = 25, Quantity = 8, UnitPrice = 54.00m, DiscountPercent = 3.0m, LineTotal = 419.04m, Notes = "Check stock before confirmation" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001027"], ProductId = 23, Quantity = 4, UnitPrice = 29.00m, DiscountPercent = 3.0m, LineTotal = 112.52m, Notes = "Check stock before confirmation" },

                // ORD-2026-001028
                new OrderItem { OrderId = orderLookup["ORD-2026-001028"], ProductId = 15, Quantity = 3, UnitPrice = 38.00m, DiscountPercent = 3.0m, LineTotal = 110.58m, Notes = null },
                new OrderItem { OrderId = orderLookup["ORD-2026-001028"], ProductId = 24, Quantity = 5, UnitPrice = 51.00m, DiscountPercent = 12.0m, LineTotal = 224.40m, Notes = "Customer requested standard packaging" },

                // ORD-2026-001029
                new OrderItem { OrderId = orderLookup["ORD-2026-001029"], ProductId = 34, Quantity = 5, UnitPrice = 79.00m, DiscountPercent = 12.0m, LineTotal = 347.60m, Notes = "ADR packaging required" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001029"], ProductId = 35, Quantity = 6, UnitPrice = 64.00m, DiscountPercent = 3.0m, LineTotal = 372.48m, Notes = "Check stock before confirmation" },

                // ORD-2026-001030
                new OrderItem { OrderId = orderLookup["ORD-2026-001030"], ProductId = 41, Quantity = 12, UnitPrice = 67.00m, DiscountPercent = 7.5m, LineTotal = 743.70m, Notes = "Check stock before confirmation" },
                new OrderItem { OrderId = orderLookup["ORD-2026-001030"], ProductId = 22, Quantity = 6, UnitPrice = 15.00m, DiscountPercent = 0.0m, LineTotal = 90.00m, Notes = null }
            };

            await _dbContext.OrderItems.AddRangeAsync(orderItems);
            await _dbContext.SaveChangesAsync();
        }

        // =========================
        // X. Order Status History
        // =========================
        if (!await _dbContext.OrderStatusHistories.AnyAsync())
        {
            var historyRows = new List<OrderStatusHistory>
            {
                // ORD-2026-001001 to ORD-2026-001010 (Draft)
                new OrderStatusHistory { OrderId = 1, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 4, ChangedAt = new DateTime(2025, 12, 16, 14, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 2, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 3, ChangedAt = new DateTime(2025, 12, 18, 7, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 3, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 3, ChangedAt = new DateTime(2025, 12, 17, 20, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 4, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 5, ChangedAt = new DateTime(2025, 12, 16, 14, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 5, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 4, ChangedAt = new DateTime(2025, 12, 17, 20, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 6, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 4, ChangedAt = new DateTime(2025, 12, 18, 2, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 7, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 5, ChangedAt = new DateTime(2025, 12, 17, 17, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 8, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 5, ChangedAt = new DateTime(2025, 12, 17, 11, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 9, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 4, ChangedAt = new DateTime(2025, 12, 17, 2, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 10, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 1, ChangedAt = new DateTime(2025, 12, 18, 6, 30, 0), Reason = null },

                // ORD-2026-001011 to ORD-2026-001020 (Submitted)
                new OrderStatusHistory { OrderId = 11, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 4, ChangedAt = new DateTime(2025, 12, 17, 21, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 11, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 8, ChangedAt = new DateTime(2025, 12, 17, 23, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 12, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 4, ChangedAt = new DateTime(2025, 12, 17, 15, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 12, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 17, 17, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 13, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 3, ChangedAt = new DateTime(2025, 12, 17, 5, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 13, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 17, 7, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 14, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 6, ChangedAt = new DateTime(2025, 12, 17, 22, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 14, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 18, 0, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 15, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 17, 23, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 15, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 18, 1, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 16, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 3, ChangedAt = new DateTime(2025, 12, 17, 5, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 16, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 17, 7, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 17, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 17, 20, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 17, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 10, ChangedAt = new DateTime(2025, 12, 17, 22, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 18, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 17, 15, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 18, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 8, ChangedAt = new DateTime(2025, 12, 17, 17, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 19, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 17, 11, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 19, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 17, 13, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 20, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 1, ChangedAt = new DateTime(2025, 12, 16, 12, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 20, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 16, 14, 30, 0), Reason = null },

                // ORD-2026-001021 to ORD-2026-001025 (Pending Review)
                new OrderStatusHistory { OrderId = 21, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 6, ChangedAt = new DateTime(2025, 12, 16, 16, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 21, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 16, 18, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 21, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 8, ChangedAt = new DateTime(2025, 12, 17, 9, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 22, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 5, ChangedAt = new DateTime(2025, 12, 16, 18, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 22, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 16, 20, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 22, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 8, ChangedAt = new DateTime(2025, 12, 18, 0, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 23, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 5, ChangedAt = new DateTime(2025, 12, 16, 18, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 23, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 16, 20, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 23, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 18, 12, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 24, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 6, ChangedAt = new DateTime(2025, 12, 17, 3, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 24, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 17, 5, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 24, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 17, 11, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 25, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 1, ChangedAt = new DateTime(2025, 12, 16, 13, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 25, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 10, ChangedAt = new DateTime(2025, 12, 16, 15, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 25, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 1, ChangedAt = new DateTime(2025, 12, 17, 13, 30, 0), Reason = null },

                // ORD-2026-001026 to ORD-2026-001027 (Approved)
                new OrderStatusHistory { OrderId = 26, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 17, 2, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 26, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 17, 4, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 26, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 18, 9, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 26, FromStatusId = 3, ToStatusId = 4, ChangedByUserId = 10, ChangedAt = new DateTime(2025, 12, 18, 9, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 27, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 3, ChangedAt = new DateTime(2025, 12, 17, 16, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 27, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 17, 18, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 27, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 1, ChangedAt = new DateTime(2025, 12, 17, 13, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 27, FromStatusId = 3, ToStatusId = 4, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 17, 3, 30, 0), Reason = null },

                // ORD-2026-001028 to ORD-2026-001029 (In Processing)
                new OrderStatusHistory { OrderId = 28, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 16, 14, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 28, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 16, 16, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 28, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 17, 11, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 28, FromStatusId = 3, ToStatusId = 4, ChangedByUserId = 7, ChangedAt = new DateTime(2025, 12, 18, 23, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 28, FromStatusId = 4, ToStatusId = 5, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 18, 18, 30, 0), Reason = null },

                new OrderStatusHistory { OrderId = 29, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 2, ChangedAt = new DateTime(2025, 12, 17, 0, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 29, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 8, ChangedAt = new DateTime(2025, 12, 17, 2, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 29, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 9, ChangedAt = new DateTime(2025, 12, 18, 12, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 29, FromStatusId = 3, ToStatusId = 4, ChangedByUserId = 10, ChangedAt = new DateTime(2025, 12, 18, 22, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 29, FromStatusId = 4, ToStatusId = 5, ChangedByUserId = 8, ChangedAt = new DateTime(2025, 12, 19, 0, 30, 0), Reason = null },

                // ORD-2026-001030
                new OrderStatusHistory { OrderId = 30, FromStatusId = null, ToStatusId = 1, ChangedByUserId = 6, ChangedAt = new DateTime(2025, 12, 18, 1, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 30, FromStatusId = 1, ToStatusId = 2, ChangedByUserId = 10, ChangedAt = new DateTime(2025, 12, 18, 3, 30, 0), Reason = null },
                new OrderStatusHistory { OrderId = 30, FromStatusId = 2, ToStatusId = 3, ChangedByUserId = 11, ChangedAt = new DateTime(2025, 12, 18, 7, 30, 0), Reason = null }
            };

            await _dbContext.OrderStatusHistories.AddRangeAsync(historyRows);
            await _dbContext.SaveChangesAsync();
        }

        // Processing jobs are runtime workflow records.
        // They are intentionally not seeded so the background processor remains the source of truth.

        // =========================
        // X. Audit Logs
        // =========================
        if (!await _dbContext.AuditLogs.AnyAsync())
        {
            var ordersByNumber = await _dbContext.Orders
                .ToDictionaryAsync(o => o.OrderNumber, o => o.OrderId);

            var auditLogs = new List<AuditLog>
            {
                new AuditLog
                {
                    EntityType = "Order",
                    EntityId = ordersByNumber["ORD-2026-001001"],
                    Action = "Created",
                    PerformedByUserId = 4,
                    PerformedAt = new DateTime(2025, 12, 16, 14, 30, 0),
                    OldValuesJson = null,
                    NewValuesJson = "{\"orderNumber\":\"ORD-2026-001001\",\"status\":\"Draft\"}",
                    Notes = "Order record created."
                },
                new AuditLog
                {
                    EntityType = "Order",
                    EntityId = ordersByNumber["ORD-2026-001001"],
                    Action = "StatusChanged:Draft",
                    PerformedByUserId = 4,
                    PerformedAt = new DateTime(2025, 12, 16, 14, 30, 0),
                    OldValuesJson = null,
                    NewValuesJson = "{\"status\":\"Draft\"}",
                    Notes = "Workflow moved to Draft."
                },
                new AuditLog
                {
                    EntityType = "Order",
                    EntityId = ordersByNumber["ORD-2026-001011"],
                    Action = "StatusChanged:Submitted",
                    PerformedByUserId = 4,
                    PerformedAt = new DateTime(2025, 12, 17, 23, 30, 0),
                    OldValuesJson = "{\"status\":\"Draft\"}",
                    NewValuesJson = "{\"status\":\"Submitted\"}",
                    Notes = "Sales submitted the order for review."
                }
            };

            await _dbContext.AuditLogs.AddRangeAsync(auditLogs);
            await _dbContext.SaveChangesAsync();
        }
        // =========================
        // X. Notifications
        // =========================
        if (!await _dbContext.Notifications.AnyAsync())
        {
            var ordersByNumber = await _dbContext.Orders
                .ToDictionaryAsync(o => o.OrderNumber, o => o.OrderId);

            static Notification CreateNotification(
                Dictionary<string, int> ordersByNumber,
                string orderNumber,
                string email,
                string type,
                DateTime createdAt,
                string status,
                DateTime? sentAt = null)
            {
                return new Notification
                {
                    OrderId = ordersByNumber[orderNumber],
                    RecipientEmail = email,
                    NotificationType = type,
                    Subject = $"Confirmation for {orderNumber}",
                    CreatedAt = createdAt,
                    SentAt = sentAt,
                    Status = status,
                    FailureReason = null
                };
            }

            var notifications = new List<Notification>
            {
                // =========================
                // ORD-2026-001011
                // =========================
                CreateNotification(ordersByNumber, "ORD-2026-001011", "purchasing11@riverlinepackaging.co.uk", "OrderSubmitted",
                    new DateTime(2025, 12, 18, 0, 25, 0), "Sent",
                    new DateTime(2025, 12, 18, 0, 30, 0)),

                CreateNotification(ordersByNumber, "ORD-2026-001011", "purchasing22@granthampackaging.co.uk", "OrderSubmitted",
                    new DateTime(2025, 12, 17, 18, 25, 0), "Sent",
                    new DateTime(2025, 12, 17, 18, 30, 0)),

                // =========================
                // ORD-2026-001012
                // =========================
                CreateNotification(ordersByNumber, "ORD-2026-001012", "purchasing10@northernfoodprocess.co.uk", "OrderSubmitted",
                    new DateTime(2025, 12, 17, 8, 20, 0), "Sent",
                    new DateTime(2025, 12, 17, 8, 30, 0)),

                CreateNotification(ordersByNumber, "ORD-2026-001012", "purchasing3@alderleyanalytical.co.uk", "OrderSubmitted",
                    new DateTime(2025, 12, 18, 1, 20, 0), "Sent",
                    new DateTime(2025, 12, 18, 1, 30, 0)),

                // =========================
                // ORD-2026-001013 (queued)
                // =========================
                CreateNotification(ordersByNumber, "ORD-2026-001013", "purchasing5@redbrickmanufacturing.co.uk", "OrderSubmitted",
                    new DateTime(2025, 12, 17, 8, 25, 0), "Queued"),

                CreateNotification(ordersByNumber, "ORD-2026-001013", "purchasing8@seftonfacilities.co.uk", "OrderSubmitted",
                    new DateTime(2025, 12, 17, 23, 25, 0), "Queued"),

                // Runtime approval/completion notifications are intentionally not seeded.
                // They are created by the background processor when orders move through the workflow.
            };

            await _dbContext.Notifications.AddRangeAsync(notifications);
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

