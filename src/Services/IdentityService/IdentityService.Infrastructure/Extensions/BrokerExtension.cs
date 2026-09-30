using IdentityService.Infrastructure.Broker.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Extensions;

public static class BrokerExtension
{
    public static IServiceCollection AddBrokerServices(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<GetOrCreateCustomerConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}