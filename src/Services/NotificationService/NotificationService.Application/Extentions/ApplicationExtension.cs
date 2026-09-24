using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Application.Services;

namespace NotificationService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITicketAssignedNotificationService, TicketAssignedNotificationService>();

            return services;
        }
    }
}