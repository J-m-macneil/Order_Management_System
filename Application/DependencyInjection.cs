using Application.Common.Services;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuditChangeFormatter, AuditChangeFormatter>();
        services.AddScoped<IOrderReviewPolicy, OrderReviewPolicy>();

        return services;
    }
}
