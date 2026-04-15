using System;
using System.Text;
using System.Threading.Tasks;
using Application;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .WithOrigins("https://localhost:53923")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);


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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("SalesOrAdmin", policy =>
                policy.RequireRole("Sales", "Admin"));

            options.AddPolicy("OperationsOrAdmin", policy =>
                policy.RequireRole("Operations", "Admin"));
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .WithOrigins("https://localhost:53923")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.MapStaticAssets();

        // Pipeline
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

        await app.RunAsync();
    }
}