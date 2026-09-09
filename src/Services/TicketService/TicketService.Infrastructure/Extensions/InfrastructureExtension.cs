using Microsoft.Extensions.DependencyInjection;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Infrastructure.Data;
using TicketService.Infrastructure.Repositories;
using TicketService.Infrastructure.Workers;

namespace TicketService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITicketRepository, TicketRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAgentRepository, AgentRepository>();

        services.AddScoped<IAssignmentAttemptRepository, AssignmentAttemptRepository>();

        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        services.AddHostedService<RetryAssignmentWorker>();

        return services;
    }
}