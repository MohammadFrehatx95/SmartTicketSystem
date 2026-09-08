using Microsoft.Extensions.DependencyInjection;
using TicketService.Application.Interfaces.Services;
using TicketService.Application.Services;

namespace TicketService.Application.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICreateTicketService, CreateTicketService>();

        services.AddScoped<IAssignTicketService, AssignTicketService>();

        services.AddScoped<ICreateAgentService, CreateAgentService>();

        services.AddScoped<IAutoAssignTicketService, AutoAssignTicketService>();

        return services;
    }
}