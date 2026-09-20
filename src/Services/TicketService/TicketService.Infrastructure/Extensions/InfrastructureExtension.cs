using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Infrastructure.BackgroundJobs;
using TicketService.Infrastructure.Data;
using TicketService.Infrastructure.Options;
using TicketService.Infrastructure.Repositories;

namespace TicketService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITicketRepository, TicketRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAgentRepository, AgentRepository>();

        services.AddScoped<IAssignmentAttemptRepository, AssignmentAttemptRepository>();

        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        services.Configure<RetryWorkerOption>(configuration.GetSection("RetryWorker"));

        services.Configure<RetryWorkerOption>(configuration.GetSection("RetryWorker"));

        var intervalMinutes = configuration.GetValue<int>("RetryWorker:IntervalMinutes", 60);

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("RetryAssignmentJob");

            q.AddJob<RetryAssignmentJob>(options => options.WithIdentity(jobKey));

            q.AddTrigger<RetryAssignmentJob>(options => options
                .ForJob(jobKey)
                .WithIdentity("RetryAssignmentJob-Trigger")
                .StartNow()
                .WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromMinutes(intervalMinutes)).RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}