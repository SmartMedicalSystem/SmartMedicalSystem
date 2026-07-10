using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataSeed
{
    /// <summary>
    /// Applies pending migrations and seeds roles, permissions, role-permission
    /// mappings, and a default Admin account. Call once at startup, e.g. from
    /// Program.cs:
    ///
    ///   using (var scope = app.Services.CreateScope())
    ///   {
    ///       await DbInitializer.SeedAsync(scope.ServiceProvider);
    ///   }
    /// </summary>
    public static class DbInitializer
    {
        public const string DefaultAdminEmail = "admin@medsystem.local";
        public const string DefaultAdminPassword = "Admin@12345";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            await RoleSeeder.SeedAsync(roleManager);
            await PermissionSeeder.SeedAsync(context);
            await RolePermissionSeeder.SeedAsync(context);

            await SeedAdminUserAsync(userManager);
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync(DefaultAdminEmail);
            if (adminUser is not null)
                return;

            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = DefaultAdminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };

            // ASSUMPTION / TODO: placeholder password purely so the seeded account
            // satisfies Identity's default password rules. Change it before any
            // real deployment (move to configuration/user-secrets).
            var result = await userManager.CreateAsync(adminUser, DefaultAdminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
            }
        }
    }
}
