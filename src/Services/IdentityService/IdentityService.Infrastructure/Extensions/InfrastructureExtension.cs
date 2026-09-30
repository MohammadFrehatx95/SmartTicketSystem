using IdentityService.Application.Interfaces.Persistence;
using IdentityService.Application.Interfaces.Repositiories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Application.Services;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityServices>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<ICustomerService, CustomerService>();

        services.AddScoped<ICustomerUnitOfWork, CustomerUnitOfWork>();

        return services;
    }
}