using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Broker;
using Shared.Infrastructure.Broker;

namespace TicketService.Infrastructure.Extensions
{
    public static class BrokerExtensions
    {
        public static IServiceCollection AddBroker(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], h =>
                    {
                        h.Username(configuration["RabbitMq:Username"]!);
                        h.Password(configuration["RabbitMq:Password"]!);
                    });
                });
            });

            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

            return services;
        }
    }
}