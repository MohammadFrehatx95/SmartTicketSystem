using IdentityService.Application.Constants;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Identity.Seeding;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<long>> roleManager)
    {
        var roles = new[]
        {
            RoleNames.Admin,
            RoleNames.Agent,
            RoleNames.Customer
        };

        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);

            if (!roleExists)
                await roleManager.CreateAsync(new IdentityRole<long>(role));
        }
    }
}