using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    CarrierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ServiceType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.CarrierId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "HazardClasses",
                columns: table => new
                {
                    HazardClassId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HazardClasses", x => x.HazardClassId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsTerminal = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "PricingTiers",
                columns: table => new
                {
                    PricingTierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    PriorityProcessing = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingTiers", x => x.PricingTierId);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SystemSettingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SettingKey = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    SettingValue = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DataType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SystemSettingId);
                });

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasure",
                columns: table => new
                {
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasure", x => x.UnitOfMeasureId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    FullName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SKU = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProductCategoryId = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: false),
                    PackSize = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    HazardClassId = table.Column<int>(type: "integer", nullable: false),
                    UNNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StorageRequirement = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    RequiresSds = table.Column<bool>(type: "boolean", nullable: false),
                    IsRestricted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_HazardClasses_HazardClassId",
                        column: x => x.HazardClassId,
                        principalTable: "HazardClasses",
                        principalColumn: "HazardClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_UnitsOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "UnitOfMeasureId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    PerformedByUserId = table.Column<int>(type: "integer", nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OldValuesJson = table.Column<string>(type: "text", nullable: true),
                    NewValuesJson = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SafetyDataSheets",
                columns: table => new
                {
                    SafetyDataSheetId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "date", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedByUserId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyDataSheets", x => x.SafetyDataSheetId);
                    table.ForeignKey(
                        name: "FK_SafetyDataSheets_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SafetyDataSheets_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    AddressType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SiteName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Line1 = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Line2 = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    City = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    County = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Postcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ContactName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DeliveryInstructions = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IndustryType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    MainContactName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    MainContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MainContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BillingAddressId = table.Column<int>(type: "integer", nullable: true),
                    DefaultDeliveryAddressId = table.Column<int>(type: "integer", nullable: true),
                    PricingTierId = table.Column<int>(type: "integer", nullable: false),
                    PaymentTermsDays = table.Column<int>(type: "integer", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Addresses_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_Addresses_DefaultDeliveryAddressId",
                        column: x => x.DefaultDeliveryAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_PricingTiers_PricingTierId",
                        column: x => x.PricingTierId,
                        principalTable: "PricingTiers",
                        principalColumn: "PricingTierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    ContactName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseId);
                    table.ForeignKey(
                        name: "FK_Warehouses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerContacts",
                columns: table => new
                {
                    CustomerContactId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerContacts", x => x.CustomerContactId);
                    table.ForeignKey(
                        name: "FK_CustomerContacts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerProductPrices",
                columns: table => new
                {
                    CustomerProductPriceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    OverridePrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    MinimumOrderQuantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProductPrices", x => x.CustomerProductPriceId);
                    table.ForeignKey(
                        name: "FK_CustomerProductPrices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    ProjectCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    DeliveryAddressId = table.Column<int>(type: "integer", nullable: false),
                    BillingAddressId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    CarrierId = table.Column<int>(type: "integer", nullable: true),
                    OrderStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    RequestedDeliveryDate = table.Column<DateTime>(type: "date", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    PurchaseOrderReference = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    SpecialInstructions = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPriorityOrder = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_DeliveryAddressId",
                        column: x => x.DeliveryAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Documents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    RecipientEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Subject = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusHistories",
                columns: table => new
                {
                    OrderStatusHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    FromStatusId = table.Column<int>(type: "integer", nullable: true),
                    ToStatusId = table.Column<int>(type: "integer", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistories", x => x.OrderStatusHistoryId);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_OrderStatuses_FromStatusId",
                        column: x => x.FromStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_OrderStatuses_ToStatusId",
                        column: x => x.ToStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingJobs",
                columns: table => new
                {
                    ProcessingJobId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    JobType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRetryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextAttemptAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PayloadJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJobs", x => x.ProcessingJobId);
                    table.ForeignKey(
                        name: "FK_ProcessingJobs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Carriers",
                columns: new[] { "CarrierId", "ContactEmail", "IsActive", "Name", "ServiceType" },
                values: new object[,]
                {
                    { 1, "ops@northhaul.co.uk", true, "NorthHaul Logistics", "ADR / General Haulage" },
                    { 2, "bookings@merseyfreight.co.uk", true, "Mersey Freight Partners", "Regional Pallet and Drum Delivery" },
                    { 3, "orders@chemsafe-transport.co.uk", true, "ChemSafe Transport", "Hazardous Goods Specialist" },
                    { 4, "dispatch@westline.co.uk", false, "WestLine Distribution", "General Commercial Distribution" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Name" },
                values: new object[,]
                {
                    { 1, "IT" },
                    { 2, "Sales" },
                    { 3, "Operations" },
                    { 4, "Customer Service" },
                    { 5, "Finance" }
                });

            migrationBuilder.InsertData(
                table: "HazardClasses",
                columns: new[] { "HazardClassId", "Name" },
                values: new object[,]
                {
                    { 1, "Non-Hazardous" },
                    { 2, "Flammable" },
                    { 3, "Corrosive" },
                    { 4, "Toxic" },
                    { 5, "Oxidising" },
                    { 6, "Irritant" },
                    { 7, "Environmental Hazard" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "OrderStatusId", "DisplayOrder", "IsTerminal", "Name" },
                values: new object[,]
                {
                    { 1, 1, false, "Draft" },
                    { 2, 2, false, "Submitted" },
                    { 3, 3, false, "Pending Review" },
                    { 4, 4, false, "Approved" },
                    { 5, 5, false, "In Processing" },
                    { 6, 6, false, "Awaiting Dispatch" },
                    { 7, 7, true, "Completed" },
                    { 8, 8, true, "Failed" },
                    { 9, 9, true, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "PricingTiers",
                columns: new[] { "PricingTierId", "Description", "DiscountPercent", "Name", "PriorityProcessing" },
                values: new object[,]
                {
                    { 1, "Default commercial terms and standard processing.", 0.00m, "Standard", false },
                    { 2, "Low discount for steady-volume accounts.", 3.00m, "Silver", false },
                    { 3, "Higher discount and faster handling for key accounts.", 7.50m, "Gold", true },
                    { 4, "Priority customers with strong commercial terms.", 12.00m, "Strategic", true },
                    { 5, "Customer-specific contract pricing by product.", 15.00m, "Contract", true }
                });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "ProductCategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Solvent-based products used in cleaning, coatings and laboratory operations.", "Solvents" },
                    { 2, "Acidic products used in treatment, descaling and process control.", "Acids" },
                    { 3, "Alkaline products used for cleaning, pH control and industrial operations.", "Alkalis" },
                    { 4, "Products used in wastewater, potable water and process water treatment.", "Water Treatment" },
                    { 5, "General industrial and specialist cleaning solutions.", "Cleaning Chemicals" },
                    { 6, "Reagents and calibration liquids for lab environments.", "Laboratory Reagents" },
                    { 7, "Products suitable for food and beverage environments.", "Food-Safe" },
                    { 8, "Supporting consumables and handling items.", "Consumables" },
                    { 9, "Additives, agents and specialist blends.", "Industrial Additives" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Sales" },
                    { 3, "Operations" },
                    { 4, "Demo" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "SystemSettingId", "CreatedAt", "DataType", "Description", "SettingKey", "SettingValue", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "integer", "Default VAT rate used in order total calculations.", "DefaultTaxRate", "20", null },
                    { 2, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "boolean", "Whether priority flagging is enabled in the order workflow.", "EnablePriorityOrders", "true", null },
                    { 3, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "boolean", "Whether low-value orders can bypass manual review.", "AutoApproveLowValueOrders", "false", null },
                    { 4, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "integer", "Maximum number of retry attempts for background processing jobs.", "BackgroundJobRetryLimit", "3", null },
                    { 5, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "integer", "Default date window used for the operational dashboard.", "DashboardDefaultDays", "30", null },
                    { 6, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "boolean", "Whether SDS metadata is mandatory for hazardous or restricted products.", "RequireSdsForHazardousProducts", "true", null }
                });

            migrationBuilder.InsertData(
                table: "UnitsOfMeasure",
                columns: new[] { "UnitOfMeasureId", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "L", "Litre" },
                    { 2, "KG", "Kilogram" },
                    { 3, "DRUM", "Drum" },
                    { 4, "PACK", "Pack" },
                    { 5, "BOTTLE", "Bottle" },
                    { 6, "IBC", "IBC" },
                    { 7, "BAG", "Bag" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "DepartmentId", "Email", "FirstName", "FullName", "IsActive", "JobTitle", "LastLoginAt", "LastName", "PasswordHash", "RoleId", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 17, 9, 0, 0, 0, DateTimeKind.Utc), 1, "sarah.bennett@chemflow.local", "Sarah", "Sarah Bennett", true, "Head of Business Systems", new DateTime(2026, 3, 21, 8, 15, 0, 0, DateTimeKind.Utc), "Bennett", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 1, "sbennett" },
                    { 2, new DateTime(2024, 1, 29, 9, 0, 0, 0, DateTimeKind.Utc), 1, "james.carter@chemflow.local", "James", "James Carter", true, "IT Systems Administrator", new DateTime(2026, 4, 10, 3, 15, 0, 0, DateTimeKind.Utc), "Carter", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 1, "jcarter" },
                    { 3, new DateTime(2024, 2, 10, 9, 0, 0, 0, DateTimeKind.Utc), 2, "olivia.hughes@chemflow.local", "Olivia", "Olivia Hughes", true, "Account Manager", new DateTime(2026, 4, 2, 7, 15, 0, 0, DateTimeKind.Utc), "Hughes", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 2, "ohughes" },
                    { 4, new DateTime(2024, 2, 22, 9, 0, 0, 0, DateTimeKind.Utc), 2, "daniel.foster@chemflow.local", "Daniel", "Daniel Foster", true, "Internal Sales Executive", new DateTime(2026, 4, 3, 7, 15, 0, 0, DateTimeKind.Utc), "Foster", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 2, "dfoster" },
                    { 5, new DateTime(2024, 3, 5, 9, 0, 0, 0, DateTimeKind.Utc), 2, "megan.patel@chemflow.local", "Megan", "Megan Patel", true, "Sales Coordinator", new DateTime(2026, 3, 18, 8, 15, 0, 0, DateTimeKind.Utc), "Patel", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 2, "mpatel" },
                    { 6, new DateTime(2024, 3, 17, 9, 0, 0, 0, DateTimeKind.Utc), 2, "thomas.green@chemflow.local", "Thomas", "Thomas Green", true, "Regional Sales Representative", new DateTime(2026, 3, 20, 3, 15, 0, 0, DateTimeKind.Utc), "Green", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 2, "tgreen" },
                    { 7, new DateTime(2024, 3, 29, 9, 0, 0, 0, DateTimeKind.Utc), 3, "rachel.morgan@chemflow.local", "Rachel", "Rachel Morgan", true, "Operations Planner", new DateTime(2026, 3, 24, 8, 15, 0, 0, DateTimeKind.Utc), "Morgan", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 3, "rmorgan" },
                    { 8, new DateTime(2024, 4, 10, 9, 0, 0, 0, DateTimeKind.Utc), 3, "ben.turner@chemflow.local", "Ben", "Ben Turner", true, "Logistics Coordinator", new DateTime(2026, 3, 23, 5, 15, 0, 0, DateTimeKind.Utc), "Turner", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 3, "bturner" },
                    { 9, new DateTime(2024, 4, 22, 9, 0, 0, 0, DateTimeKind.Utc), 3, "emily.scott@chemflow.local", "Emily", "Emily Scott", true, "Order Processing Specialist", new DateTime(2026, 4, 9, 8, 15, 0, 0, DateTimeKind.Utc), "Scott", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 3, "escott" },
                    { 10, new DateTime(2024, 5, 4, 9, 0, 0, 0, DateTimeKind.Utc), 3, "nathan.price@chemflow.local", "Nathan", "Nathan Price", true, "Warehouse & Dispatch Coordinator", new DateTime(2026, 4, 8, 7, 15, 0, 0, DateTimeKind.Utc), "Price", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 3, "nprice" },
                    { 11, new DateTime(2024, 5, 16, 9, 0, 0, 0, DateTimeKind.Utc), 3, "chloe.evans@chemflow.local", "Chloe", "Chloe Evans", true, "Customer Fulfilment Analyst", new DateTime(2026, 4, 3, 4, 15, 0, 0, DateTimeKind.Utc), "Evans", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 3, "cevans" },
                    { 12, new DateTime(2024, 5, 28, 9, 0, 0, 0, DateTimeKind.Utc), 2, "laura.jenkins@chemflow.local", "Laura", "Laura Jenkins", false, "Former Account Manager", null, "Jenkins", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 2, "ljenkins" },
                    { 13, new DateTime(2024, 6, 9, 9, 0, 0, 0, DateTimeKind.Utc), 3, "matthew.collins@chemflow.local", "Matthew", "Matthew Collins", false, "Former Logistics Coordinator", null, "Collins", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 3, "mcollins" },
                    { 14, new DateTime(2024, 6, 21, 9, 0, 0, 0, DateTimeKind.Utc), 1, "sophie.ward@chemflow.local", "Sophie", "Sophie Ward", false, "Former Systems Analyst", null, "Ward", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 1, "sward" },
                    { 15, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), 1, "demo@back.software", "Demo", "Demo User", true, "Read-only Demonstration Account", null, "User", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 4, "demo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_Name",
                table: "Carriers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_CustomerId",
                table: "CustomerContacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductPrices_CustomerId_ProductId_EffectiveFrom",
                table: "CustomerProductPrices",
                columns: new[] { "CustomerId", "ProductId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductPrices_ProductId",
                table: "CustomerProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountNumber",
                table: "Customers",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BillingAddressId",
                table: "Customers",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DefaultDeliveryAddressId",
                table: "Customers",
                column: "DefaultDeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PricingTierId",
                table: "Customers",
                column: "PricingTierId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedByUserId",
                table: "Documents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OrderId",
                table: "Documents",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_HazardClasses_Name",
                table: "HazardClasses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OrderId",
                table: "Notifications",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AssignedToUserId",
                table: "Orders",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BillingAddressId",
                table: "Orders",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CarrierId",
                table: "Orders",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryAddressId",
                table: "Orders",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProjectId",
                table: "Orders",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RequestedDeliveryDate",
                table: "Orders",
                column: "RequestedDeliveryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WarehouseId",
                table: "Orders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatuses_Name",
                table: "OrderStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ChangedByUserId",
                table: "OrderStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_FromStatusId",
                table: "OrderStatusHistories",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_OrderId",
                table: "OrderStatusHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ToStatusId",
                table: "OrderStatusHistories",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingTiers_Name",
                table: "PricingTiers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_OrderId",
                table: "ProcessingJobs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Name",
                table: "ProductCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_HazardClassId",
                table: "Products",
                column: "HazardClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitOfMeasureId",
                table: "Products",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CustomerId",
                table: "Projects",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCode",
                table: "Projects",
                column: "ProjectCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyDataSheets_ProductId",
                table: "SafetyDataSheets",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyDataSheets_UploadedByUserId",
                table: "SafetyDataSheets",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_SettingKey",
                table: "SystemSettings",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_Code",
                table: "UnitsOfMeasure",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_AddressId",
                table: "Warehouses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Code",
                table: "Warehouses",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CustomerContacts");

            migrationBuilder.DropTable(
                name: "CustomerProductPrices");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderStatusHistories");

            migrationBuilder.DropTable(
                name: "ProcessingJobs");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SafetyDataSheets");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Carriers");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "HazardClasses");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasure");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "PricingTiers");
        }
    }
}
