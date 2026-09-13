using IdentityService.Infrastructure.Identity.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Extensions;

public static class IdentitySeedExtension
{
    public static async Task SeedIdentityRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

        await RoleSeeder.SeedRolesAsync(roleManager);
    }
}