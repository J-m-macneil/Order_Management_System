using Application.Interfaces;
using Domain.Repositories;
using Infrastructure.Identity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
        services.AddScoped<IDataSeeder, Persistence.Seed.DataSeeder>();

        return services;
    }
}
