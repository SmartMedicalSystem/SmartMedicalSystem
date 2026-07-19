using Domain.Entities;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class RolePermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        foreach (var role in RolePermissions.PermissionsByRole)
        {
            foreach (var permission in role.Value)
            {
                bool exists = await context.RolePermissions.AnyAsync(x =>
                    x.RoleId == (int)role.Key &&
                    x.PermissionId == (int)permission);

                if (!exists)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = (int)role.Key,
                        PermissionId = (int)permission
                    });
                }
            }
        }

        await context.SaveChangesAsync();
    }
}