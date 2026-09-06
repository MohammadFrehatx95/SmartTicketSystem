using System.Text.Json.Serialization;

namespace TicketService.Api.Extensions
{
    public static class ApiExtention
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
