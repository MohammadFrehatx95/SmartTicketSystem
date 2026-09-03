using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection ConnectDataBase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TicketDatabase");

            services.AddDbContext<TicketDbContext>(options => 
               options.UseSqlServer(connectionString));

            return services;
        }
    }
}
