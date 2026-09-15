using IdentityService.Application.Interfaces.Services;
using IdentityService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityServices>();

        services.AddScoped<IJwtService, JwtService>();


        return services;
    }
}