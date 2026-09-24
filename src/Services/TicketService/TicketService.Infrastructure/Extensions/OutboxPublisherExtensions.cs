using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TicketService.Infrastructure.BackgroundJobs;
using TicketService.Infrastructure.Options;

namespace TicketService.Infrastructure.Extensions;

public static class OutboxPublisherExtensions
{
    public static IServiceCollection AddOutboxPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxPublisherOptions>(configuration.GetSection("OutboxPublisher"));

        var options = configuration.GetSection("OutboxPublisher").Get<OutboxPublisherOptions>() ?? new OutboxPublisherOptions();

        var jobKey = new JobKey(nameof(OutboxPublisherJob));

        services.AddQuartz(q =>
        {
            q.AddJob<OutboxPublisherJob>(job => job.WithIdentity(jobKey));

            q.AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithIdentity($"{nameof(OutboxPublisherJob)}Trigger")
                .StartNow()
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(options.IntervalSeconds))
                    .RepeatForever()));
        });

        return services;
    }
}