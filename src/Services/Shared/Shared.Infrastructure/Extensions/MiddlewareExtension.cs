using Microsoft.AspNetCore.Builder;
using Shared.Infrastructure.Middlewares;

namespace Shared.Infrastructure.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseSharedMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }
}