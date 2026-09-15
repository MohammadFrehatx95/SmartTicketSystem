using IdentityService.Application.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure.Identity.Seeding
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string email = "admin@smartticket.com";

            var admin = await userManager.FindByEmailAsync(email);

            if (admin is not null)
                return;

            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = email,
                FirstName = "System",
                LastName = "Admin",
                NationalNumber = "ADMIN-001",
                BirthDate = new DateTime(1995, 1, 1)
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (!result.Succeeded)
                throw new Exception(result.Errors.FirstOrDefault()?.Description ?? "Admin creation failed.");

            await userManager.AddToRoleAsync(admin,RoleNames.Admin);
        }
    }
}
