using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Broker.Consumers;
using Shared.Application.Events;

namespace NotificationService.Infrastructure.Extensions
{
    public static class BrokerExtensions
    {
        public static IServiceCollection AddBroker(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TicketAssignedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], h =>
                    {
                        h.Username(configuration["RabbitMq:Username"]!);
                        h.Password(configuration["RabbitMq:Password"]!);
                    });

                    cfg.ReceiveEndpoint(nameof(TicketAssignedEvent), e =>
                    {
                        e.ConfigureConsumer<TicketAssignedConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}