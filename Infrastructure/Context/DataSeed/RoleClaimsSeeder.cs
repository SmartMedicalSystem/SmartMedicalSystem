using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Domain.Identity;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Domain.Constants;

namespace Infrastructure.DataSeed;

public static class RoleClaimsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
    {
        // Seed role claims solely from the code-defined RolePermissions.PermissionsByRole mapping.
        // This is idempotent: existing claims are checked before adding.

        foreach (var kvp in RolePermissions.PermissionsByRole)
        {
            var roleName = kvp.Key.ToString();
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                continue;

            var existingClaims = await roleManager.GetClaimsAsync(role);

            foreach (var permission in kvp.Value)
            {
                var permissionName = permission.ToString();

                if (!existingClaims.Any(c => c.Type == CustomClaimTypes.Permission && c.Value == permissionName))
                {
                    await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permissionName));
                }
            }
        }
    }
}
