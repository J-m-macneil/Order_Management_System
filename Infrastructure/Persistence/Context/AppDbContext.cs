using Domain.Entities;
using Domain.Entities.Customers;
using Domain.Entities.Orders;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PricingTier> PricingTiers => Set<PricingTier>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<HazardClass> HazardClasses => Set<HazardClass>();
    public DbSet<SafetyDataSheet> SafetyDataSheets => Set<SafetyDataSheet>();
    public DbSet<CustomerProductPrice> CustomerProductPrices => Set<CustomerProductPrice>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<ProcessingJob> ProcessingJobs => Set<ProcessingJob>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemSetting> SystemSettings => Set <SystemSetting> ();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(x => x.RoleId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                Name = "Admin"
            },
            new Role
            {
                RoleId = 2,
                Name = "Sales"
            },
            new Role
            {
                RoleId = 3,
                Name = "Operations"
            }
        );

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.UserId);

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(160);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.Username)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.JobTitle)
                .HasMaxLength(120);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(x => x.LastLoginAt)
                .HasColumnType("datetime2");

            entity.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Department)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                FirstName = "Sarah",
                LastName = "Bennett",
                FullName = "Sarah Bennett",
                Email = "sarah.bennett@chemflow.local",
                Username = "sbennett",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 1,
                DepartmentId = 1,
                JobTitle = "Head of Business Systems",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 17, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 3, 21, 8, 15, 0)
            },
            new User
            {
                UserId = 2,
                FirstName = "James",
                LastName = "Carter",
                FullName = "James Carter",
                Email = "james.carter@chemflow.local",
                Username = "jcarter",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 1,
                DepartmentId = 1,
                JobTitle = "IT Systems Administrator",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 29, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 4, 10, 3, 15, 0)
            },
            new User
            {
                UserId = 3,
                FirstName = "Olivia",
                LastName = "Hughes",
                FullName = "Olivia Hughes",
                Email = "olivia.hughes@chemflow.local",
                Username = "ohughes",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 2,
                DepartmentId = 2,
                JobTitle = "Account Manager",
                IsActive = true,
                CreatedAt = new DateTime(2024, 2, 10, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 4, 2, 7, 15, 0)
            },
            new User
            {
                UserId = 4,
                FirstName = "Daniel",
                LastName = "Foster",
                FullName = "Daniel Foster",
                Email = "daniel.foster@chemflow.local",
                Username = "dfoster",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 2,
                DepartmentId = 2,
                JobTitle = "Internal Sales Executive",
                IsActive = true,
                CreatedAt = new DateTime(2024, 2, 22, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 4, 3, 7, 15, 0)
            },
            new User
            {
                UserId = 5,
                FirstName = "Megan",
                LastName = "Patel",
                FullName = "Megan Patel",
                Email = "megan.patel@chemflow.local",
                Username = "mpatel",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 2,
                DepartmentId = 2,
                JobTitle = "Sales Coordinator",
                IsActive = true,
                CreatedAt = new DateTime(2024, 3, 5, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 3, 18, 8, 15, 0)
            },
            new User
            {
                UserId = 6,
                FirstName = "Thomas",
                LastName = "Green",
                FullName = "Thomas Green",
                Email = "thomas.green@chemflow.local",
                Username = "tgreen",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 2,
                DepartmentId = 2,
                JobTitle = "Regional Sales Representative",
                IsActive = true,
                CreatedAt = new DateTime(2024, 3, 17, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 3, 20, 3, 15, 0)
            },
            new User
            {
                UserId = 7,
                FirstName = "Rachel",
                LastName = "Morgan",
                FullName = "Rachel Morgan",
                Email = "rachel.morgan@chemflow.local",
                Username = "rmorgan",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 3,
                DepartmentId = 3,
                JobTitle = "Operations Planner",
                IsActive = true,
                CreatedAt = new DateTime(2024, 3, 29, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 3, 24, 8, 15, 0)
            },
            new User
            {
                UserId = 8,
                FirstName = "Ben",
                LastName = "Turner",
                FullName = "Ben Turner",
                Email = "ben.turner@chemflow.local",
                Username = "bturner",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 3,
                DepartmentId = 3,
                JobTitle = "Logistics Coordinator",
                IsActive = true,
                CreatedAt = new DateTime(2024, 4, 10, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 3, 23, 5, 15, 0)
            },
            new User
            {
                UserId = 9,
                FirstName = "Emily",
                LastName = "Scott",
                FullName = "Emily Scott",
                Email = "emily.scott@chemflow.local",
                Username = "escott",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 3,
                DepartmentId = 3,
                JobTitle = "Order Processing Specialist",
                IsActive = true,
                CreatedAt = new DateTime(2024, 4, 22, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 4, 9, 8, 15, 0)
            },
            new User
            {
                UserId = 10,
                FirstName = "Nathan",
                LastName = "Price",
                FullName = "Nathan Price",
                Email = "nathan.price@chemflow.local",
                Username = "nprice",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 3,
                DepartmentId = 3,
                JobTitle = "Warehouse & Dispatch Coordinator",
                IsActive = true,
                CreatedAt = new DateTime(2024, 5, 4, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 4, 8, 7, 15, 0)
            },
            new User
            {
                UserId = 11,
                FirstName = "Chloe",
                LastName = "Evans",
                FullName = "Chloe Evans",
                Email = "chloe.evans@chemflow.local",
                Username = "cevans",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 3,
                DepartmentId = 3,
                JobTitle = "Customer Fulfilment Analyst",
                IsActive = true,
                CreatedAt = new DateTime(2024, 5, 16, 9, 0, 0),
                LastLoginAt = new DateTime(2026, 4, 3, 4, 15, 0)
            },
            new User
            {
                UserId = 12,
                FirstName = "Laura",
                LastName = "Jenkins",
                FullName = "Laura Jenkins",
                Email = "laura.jenkins@chemflow.local",
                Username = "ljenkins",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 2,
                DepartmentId = 2,
                JobTitle = "Former Account Manager",
                IsActive = false,
                CreatedAt = new DateTime(2024, 5, 28, 9, 0, 0),
                LastLoginAt = null
            },
            new User
            {
                UserId = 13,
                FirstName = "Matthew",
                LastName = "Collins",
                FullName = "Matthew Collins",
                Email = "matthew.collins@chemflow.local",
                Username = "mcollins",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 3,
                DepartmentId = 3,
                JobTitle = "Former Logistics Coordinator",
                IsActive = false,
                CreatedAt = new DateTime(2024, 6, 9, 9, 0, 0),
                LastLoginAt = null
            },
            new User
            {
                UserId = 14,
                FirstName = "Sophie",
                LastName = "Ward",
                FullName = "Sophie Ward",
                Email = "sophie.ward@chemflow.local",
                Username = "sward",
                PasswordHash = "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==",
                RoleId = 1,
                DepartmentId = 1,
                JobTitle = "Former Systems Analyst",
                IsActive = false,
                CreatedAt = new DateTime(2024, 6, 21, 9, 0, 0),
                LastLoginAt = null
            }
        );

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");

            entity.HasKey(x => x.DepartmentId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, Name = "IT" },
            new Department { DepartmentId = 2, Name = "Sales" },
            new Department { DepartmentId = 3, Name = "Operations" },
            new Department { DepartmentId = 4, Name = "Customer Service" },
            new Department { DepartmentId = 5, Name = "Finance" }
        );

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");

            entity.HasKey(x => x.CustomerId);

            entity.Property(x => x.AccountNumber)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasIndex(x => x.AccountNumber)
                .IsUnique();

            entity.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(160);

            entity.Property(x => x.IndustryType)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(x => x.MainContactName)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.MainContactEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.MainContactPhone)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.PaymentTermsDays)
                .IsRequired();

            entity.Property(x => x.CreditLimit)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.HasOne(x => x.BillingAddress)
                .WithMany()
                .HasForeignKey(x => x.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(x => x.DefaultDeliveryAddress)
                .WithMany()
                .HasForeignKey(x => x.DefaultDeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(x => x.PricingTier)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.PricingTierId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<PricingTier>(entity =>
        {
            entity.ToTable("PricingTiers");

            entity.HasKey(x => x.PricingTierId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.DiscountPercent)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            entity.Property(x => x.PriorityProcessing)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(255);
        });

        modelBuilder.Entity<PricingTier>().HasData(
            new PricingTier
            {
                PricingTierId = 1,
                Name = "Standard",
                DiscountPercent = 0.00m,
                PriorityProcessing = false,
                Description = "Default commercial terms and standard processing."
            },
            new PricingTier
            {
                PricingTierId = 2,
                Name = "Silver",
                DiscountPercent = 3.00m,
                PriorityProcessing = false,
                Description = "Low discount for steady-volume accounts."
            },
            new PricingTier
            {
                PricingTierId = 3,
                Name = "Gold",
                DiscountPercent = 7.50m,
                PriorityProcessing = true,
                Description = "Higher discount and faster handling for key accounts."
            },
            new PricingTier
            {
                PricingTierId = 4,
                Name = "Strategic",
                DiscountPercent = 12.00m,
                PriorityProcessing = true,
                Description = "Priority customers with strong commercial terms."
            },
            new PricingTier
            {
                PricingTierId = 5,
                Name = "Contract",
                DiscountPercent = 15.00m,
                PriorityProcessing = true,
                Description = "Customer-specific contract pricing by product."
            }
        );

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");

            entity.HasKey(x => x.AddressId);

            entity.Property(x => x.AddressType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SiteName)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.Line1)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.Line2)
                .HasMaxLength(120);

            entity.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(x => x.County)
                .HasMaxLength(80);

            entity.Property(x => x.Postcode)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(x => x.ContactName)
                .HasMaxLength(120);

            entity.Property(x => x.ContactPhone)
                .HasMaxLength(50);

            entity.Property(x => x.DeliveryInstructions)
                .HasMaxLength(255);

            entity.Property(x => x.IsPrimary)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouses");

            entity.HasKey(x => x.WarehouseId);

            entity.Property(x => x.Code)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.ContactName)
                .HasMaxLength(120)
                .IsRequired(false);

            entity.Property(x => x.ContactPhone)
                .HasMaxLength(50)
                .IsRequired(false);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasIndex(x => x.Code)
                .IsUnique();

            entity.HasOne(x => x.Address)
                .WithMany()
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.ToTable("CustomerContacts");

            entity.HasKey(x => x.CustomerContactId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.JobTitle)
                .HasMaxLength(120);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.Phone)
                .HasMaxLength(50);

            entity.Property(x => x.IsPrimary)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2");

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Contacts)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.ProductId);

            entity.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(40);

            entity.HasIndex(x => x.SKU)
                .IsUnique();

            entity.Property(x => x.ProductName)
                .IsRequired()
                .HasMaxLength(160);

            entity.Property(x => x.Description)
                .HasMaxLength(255);

            entity.Property(x => x.PackSize)
                .IsRequired()
                .HasMaxLength(40);

            entity.Property(x => x.BasePrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength();

            entity.Property(x => x.UNNumber)
                .HasMaxLength(20);

            entity.Property(x => x.StorageRequirement)
                .HasMaxLength(120);

            entity.Property(x => x.RequiresSds)
                .IsRequired();

            entity.Property(x => x.IsRestricted)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2");

            entity.HasOne(x => x.ProductCategory)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UnitOfMeasure)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.HazardClass)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.HazardClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategories");

            entity.HasKey(x => x.ProductCategoryId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.Description)
                .HasMaxLength(255);
        });

        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory
            {
                ProductCategoryId = 1,
                Name = "Solvents",
                Description = "Solvent-based products used in cleaning, coatings and laboratory operations."
            },
            new ProductCategory
            {
                ProductCategoryId = 2,
                Name = "Acids",
                Description = "Acidic products used in treatment, descaling and process control."
            },
            new ProductCategory
            {
                ProductCategoryId = 3,
                Name = "Alkalis",
                Description = "Alkaline products used for cleaning, pH control and industrial operations."
            },
            new ProductCategory
            {
                ProductCategoryId = 4,
                Name = "Water Treatment",
                Description = "Products used in wastewater, potable water and process water treatment."
            },
            new ProductCategory
            {
                ProductCategoryId = 5,
                Name = "Cleaning Chemicals",
                Description = "General industrial and specialist cleaning solutions."
            },
            new ProductCategory
            {
                ProductCategoryId = 6,
                Name = "Laboratory Reagents",
                Description = "Reagents and calibration liquids for lab environments."
            },
            new ProductCategory
            {
                ProductCategoryId = 7,
                Name = "Food-Safe",
                Description = "Products suitable for food and beverage environments."
            },
            new ProductCategory
            {
                ProductCategoryId = 8,
                Name = "Consumables",
                Description = "Supporting consumables and handling items."
            },
            new ProductCategory
            {
                ProductCategoryId = 9,
                Name = "Industrial Additives",
                Description = "Additives, agents and specialist blends."
            }
        );

        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.ToTable("UnitsOfMeasure");

            entity.HasKey(x => x.UnitOfMeasureId);

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(x => x.Code)
                .IsUnique();

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);
        });

        modelBuilder.Entity<UnitOfMeasure>().HasData(
            new UnitOfMeasure { UnitOfMeasureId = 1, Code = "L", Name = "Litre" },
            new UnitOfMeasure { UnitOfMeasureId = 2, Code = "KG", Name = "Kilogram" },
            new UnitOfMeasure { UnitOfMeasureId = 3, Code = "DRUM", Name = "Drum" },
            new UnitOfMeasure { UnitOfMeasureId = 4, Code = "PACK", Name = "Pack" },
            new UnitOfMeasure { UnitOfMeasureId = 5, Code = "BOTTLE", Name = "Bottle" },
            new UnitOfMeasure { UnitOfMeasureId = 6, Code = "IBC", Name = "IBC" },
            new UnitOfMeasure { UnitOfMeasureId = 7, Code = "BAG", Name = "Bag" }
        );

        modelBuilder.Entity<HazardClass>(entity =>
        {
            entity.ToTable("HazardClasses");

            entity.HasKey(x => x.HazardClassId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });

        modelBuilder.Entity<HazardClass>().HasData(
            new HazardClass { HazardClassId = 1, Name = "Non-Hazardous" },
            new HazardClass { HazardClassId = 2, Name = "Flammable" },
            new HazardClass { HazardClassId = 3, Name = "Corrosive" },
            new HazardClass { HazardClassId = 4, Name = "Toxic" },
            new HazardClass { HazardClassId = 5, Name = "Oxidising" },
            new HazardClass { HazardClassId = 6, Name = "Irritant" },
            new HazardClass { HazardClassId = 7, Name = "Environmental Hazard" }
        );

        modelBuilder.Entity<SafetyDataSheet>(entity =>
        {
            entity.ToTable("SafetyDataSheets");

            entity.HasKey(x => x.SafetyDataSheetId);

            entity.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.Version)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.EffectiveDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.UploadedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.SafetyDataSheets)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.UploadedByUser)
                .WithMany(x => x.UploadedSafetyDataSheets)
                .HasForeignKey(x => x.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<CustomerProductPrice>(entity =>
        {
            entity.ToTable("CustomerProductPrices");

            entity.HasKey(x => x.CustomerProductPriceId);

            entity.Property(x => x.OverridePrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(x => x.MinimumOrderQuantity)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(x => x.EffectiveFrom)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.EffectiveTo)
                .HasColumnType("date")
                .IsRequired(false);

            entity.Property(x => x.Notes)
                .HasMaxLength(255);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerProductPrices)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.Product)
                .WithMany(x => x.CustomerProductPrices)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(x => new { x.CustomerId, x.ProductId, x.EffectiveFrom });
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(x => x.OrderId);

            entity.Property(x => x.OrderNumber)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasIndex(x => x.OrderNumber)
                .IsUnique();

            entity.Property(x => x.RequestedDeliveryDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.SubmittedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.DeletedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(x => x.Currency)
                .HasColumnType("char(3)")
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(x => x.Subtotal)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(x => x.TaxAmount)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(x => x.TotalAmount)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(x => x.PurchaseOrderReference)
                .HasMaxLength(40)
                .IsRequired(false);

            entity.Property(x => x.SpecialInstructions)
                .HasMaxLength(255)
                .IsRequired(false);

            entity.Property(x => x.InternalNotes)
                .HasMaxLength(255)
                .IsRequired(false);

            entity.Property(x => x.FailureReason)
                .HasMaxLength(255)
                .IsRequired(false);

            entity.Property(x => x.IsPriorityOrder)
                .IsRequired();

            entity.Property(x => x.OrderStatusId)
                .HasDefaultValue(1)
                .IsRequired();

            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.ProjectId);
            entity.HasIndex(x => x.DeliveryAddressId);
            entity.HasIndex(x => x.BillingAddressId);
            entity.HasIndex(x => x.CreatedByUserId);
            entity.HasIndex(x => x.AssignedToUserId);
            entity.HasIndex(x => x.WarehouseId);
            entity.HasIndex(x => x.CarrierId);
            entity.HasIndex(x => x.OrderStatusId);
            entity.HasIndex(x => x.CreatedAt);
            entity.HasIndex(x => x.RequestedDeliveryDate);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.Project)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(x => x.DeliveryAddress)
                .WithMany(x => x.DeliveryOrders)
                .HasForeignKey(x => x.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.BillingAddress)
                .WithMany(x => x.BillingOrders)
                .HasForeignKey(x => x.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.AssignedToUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(x => x.Warehouse)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.Carrier)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CarrierId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(x => x.OrderStatus)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");

            entity.HasKey(x => x.OrderItemId);

            entity.Property(x => x.Quantity)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(x => x.UnitPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(x => x.DiscountPercent)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            entity.Property(x => x.LineTotal)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(x => x.Notes)
                .HasMaxLength(255);

            entity.HasOne(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.Product)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<ProcessingJob>(entity =>
        {
            entity.HasKey(x => x.ProcessingJobId);

            entity.Property(x => x.JobType)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(x => x.ErrorMessage)
                .HasMaxLength(500);

            entity.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Carrier>(entity =>
        {
            entity.ToTable("Carriers");

            entity.HasKey(x => x.CarrierId);

            entity.Property(x => x.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.ContactEmail)
                .HasMaxLength(255);

            entity.Property(x => x.ServiceType)
                .HasMaxLength(120);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Carrier>().HasData(
            new Carrier
            {
                CarrierId = 1,
                Name = "NorthHaul Logistics",
                ContactEmail = "ops@northhaul.co.uk",
                ServiceType = "ADR / General Haulage",
                IsActive = true
            },
            new Carrier
            {
                CarrierId = 2,
                Name = "Mersey Freight Partners",
                ContactEmail = "bookings@merseyfreight.co.uk",
                ServiceType = "Regional Pallet and Drum Delivery",
                IsActive = true
            },
            new Carrier
            {
                CarrierId = 3,
                Name = "ChemSafe Transport",
                ContactEmail = "orders@chemsafe-transport.co.uk",
                ServiceType = "Hazardous Goods Specialist",
                IsActive = true
            },
            new Carrier
            {
                CarrierId = 4,
                Name = "WestLine Distribution",
                ContactEmail = "dispatch@westline.co.uk",
                ServiceType = "General Commercial Distribution",
                IsActive = false
            }
        );

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("OrderStatuses");

            entity.HasKey(x => x.OrderStatusId);

            entity.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.IsTerminal)
                .IsRequired();

            entity.Property(x => x.DisplayOrder)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });

        modelBuilder.Entity<OrderStatus>().HasData(
            new OrderStatus
            {
                OrderStatusId = 1,
                Name = "Draft",
                IsTerminal = false,
                DisplayOrder = 1
            },
            new OrderStatus
            {
                OrderStatusId = 2,
                Name = "Submitted",
                IsTerminal = false,
                DisplayOrder = 2
            },
            new OrderStatus
            {
                OrderStatusId = 3,
                Name = "Pending Review",
                IsTerminal = false,
                DisplayOrder = 3
            },
            new OrderStatus
            {
                OrderStatusId = 4,
                Name = "Approved",
                IsTerminal = false,
                DisplayOrder = 4
            },
            new OrderStatus
            {
                OrderStatusId = 5,
                Name = "In Processing",
                IsTerminal = false,
                DisplayOrder = 5
            },
            new OrderStatus
            {
                OrderStatusId = 6,
                Name = "Awaiting Dispatch",
                IsTerminal = false,
                DisplayOrder = 6
            },
            new OrderStatus
            {
                OrderStatusId = 7,
                Name = "Completed",
                IsTerminal = true,
                DisplayOrder = 7
            },
            new OrderStatus
            {
                OrderStatusId = 8,
                Name = "Failed",
                IsTerminal = true,
                DisplayOrder = 8
            },
            new OrderStatus
            {
                OrderStatusId = 9,
                Name = "Cancelled",
                IsTerminal = true,
                DisplayOrder = 9
            }
        );

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");

            entity.HasKey(x => x.ProjectId);

            entity.Property(x => x.ProjectCode)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.ProjectName)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(255);

            entity.Property(x => x.StartDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.EndDate)
                .HasColumnType("date")
                .IsRequired(false);

            entity.Property(x => x.Status)
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(x => x.ProjectCode)
                .IsUnique();

            entity.HasIndex(x => x.CustomerId);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(x => x.AuditLogId);

            entity.Property(x => x.EntityType)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.Action)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.HasOne(x => x.PerformedByUser)
                .WithMany()
                .HasForeignKey(x => x.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(x => x.SystemSettingId);

            entity.Property(x => x.SettingKey)
                .HasMaxLength(80)
                .IsRequired();

            entity.HasIndex(x => x.SettingKey)
                .IsUnique();

            entity.Property(x => x.SettingValue)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.DataType)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(255);

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(x => x.DocumentId);

            entity.Property(x => x.DocumentType)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.FileName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.FilePath)
                .HasMaxLength(255)
                .IsRequired();

            entity.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(x => x.NotificationId);

            entity.Property(x => x.RecipientEmail)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.NotificationType)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.Subject)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(x => x.FailureReason)
                .HasMaxLength(255);

            entity.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting
            {
                SystemSettingId = 1,
                SettingKey = "DefaultTaxRate",
                SettingValue = "20",
                DataType = "integer",
                Description = "Default VAT rate used in order total calculations.",
                CreatedAt = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new SystemSetting
            {
                SystemSettingId = 2,
                SettingKey = "EnablePriorityOrders",
                SettingValue = "true",
                DataType = "boolean",
                Description = "Whether priority flagging is enabled in the order workflow.",
                CreatedAt = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new SystemSetting
            {
                SystemSettingId = 3,
                SettingKey = "AutoApproveLowValueOrders",
                SettingValue = "false",
                DataType = "boolean",
                Description = "Whether low-value orders can bypass manual review.",
                CreatedAt = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new SystemSetting
            {
                SystemSettingId = 4,
                SettingKey = "BackgroundJobRetryLimit",
                SettingValue = "3",
                DataType = "integer",
                Description = "Maximum number of retry attempts for background processing jobs.",
                CreatedAt = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new SystemSetting
            {
                SystemSettingId = 5,
                SettingKey = "DashboardDefaultDays",
                SettingValue = "30",
                DataType = "integer",
                Description = "Default date window used for the operational dashboard.",
                CreatedAt = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new SystemSetting
            {
                SystemSettingId = 6,
                SettingKey = "RequireSdsForHazardousProducts",
                SettingValue = "true",
                DataType = "boolean",
                Description = "Whether SDS metadata is mandatory for hazardous or restricted products.",
                CreatedAt = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.ToTable("OrderStatusHistories");

            entity.HasKey(x => x.OrderStatusHistoryId);

            entity.Property(x => x.ChangedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.Reason)
                .HasMaxLength(255)
                .IsRequired(false);

            entity.HasOne(x => x.Order)
                .WithMany(x => x.OrderStatusHistory)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.FromStatus)
                .WithMany()
                .HasForeignKey(x => x.FromStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(x => x.ToStatus)
                .WithMany()
                .HasForeignKey(x => x.ToStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(x => x.ChangedByUser)
                .WithMany()
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}
