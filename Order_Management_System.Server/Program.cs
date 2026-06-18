using Application;
using Application.Common.Interfaces;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Pricing.Queries;
using Application.Interfaces;
using Domain.Repositories;
using Infrastructure.DependencyInjection;
using Infrastructure.Identity;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // PDF License
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // Controllers
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy =
                    System.Text.Json.JsonNamingPolicy.CamelCase;
            });

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
        });

        // Application & Infrastructure
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        // Core Services
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<ICarrierRepository, CarrierRepository>();
        builder.Services.AddScoped<IAddressRepository, AddressRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<ICustomerContactRepository, CustomerContactRepository>();
        builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
        builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
        builder.Services.AddScoped<IFileStorageService, FileStorageService>();
        builder.Services.AddScoped<IHazardClassRepository, HazardClassRepository>();
        builder.Services.AddScoped<IProcessingJobRepository, ProcessingJobRepository>();
        builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ISafetyDataSheetRepository, SafetyDataSheetRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();

        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IPricingService, PricingService>();
        builder.Services.AddScoped<ISystemSettingsService, SystemSettingsService>();
        builder.Services.AddScoped<IAuditService, AuditService>();

        builder.Services.AddScoped<IProcessingJobQueueService, ProcessingJobQueueService>();
        builder.Services.AddScoped<IOrderDocumentGenerator, OrderDocumentGenerator>();

        // Background jobs
        builder.Services.AddHostedService<JobProcessingService>();

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins("https://localhost:53923")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // JWT Auth
        var jwtSection = builder.Configuration.GetSection("Jwt");

        var issuer = jwtSection["Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured.");

        var audience = jwtSection["Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured.");

        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userIdValue =
                            context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (!int.TryParse(userIdValue, out var userId))
                        {
                            context.Fail("Invalid token.");
                            return;
                        }

                        var dbContext = context.HttpContext.RequestServices
                            .GetRequiredService<Infrastructure.Persistence.Context.AppDbContext>();

                        var isActive = await dbContext.Users
                            .AsNoTracking()
                            .AnyAsync(x => x.UserId == userId && x.IsActive);

                        if (!isActive)
                        {
                            context.Fail("User account is inactive.");
                        }
                    }
                };
            });

        // Authorization Policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("SalesOrAdmin", policy =>
                policy.RequireRole("Sales", "Admin"));

            options.AddPolicy("OperationsOrAdmin", policy =>
                policy.RequireRole("Operations", "Admin"));
        });

        var app = builder.Build();

        // Middleware
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseDefaultFiles();
        app.MapStaticAssets();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("Frontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("/index.html");

        // Data seeding
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
            await seeder.SeedAsync();
        }

        await app.RunAsync();
    }
}
