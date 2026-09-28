using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection ConnectDataBase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotificationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}