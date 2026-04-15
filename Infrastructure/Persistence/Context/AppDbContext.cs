using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();

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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
                PasswordHash = "Password123!",
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
    }
}
