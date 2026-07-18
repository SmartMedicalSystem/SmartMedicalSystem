using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        foreach (var permission in Enum.GetValues<Permissions>())
        {
            if (!await context.Permissions.AnyAsync(p => p.Id == (int)permission))
            {
                context.Permissions.Add(new Permission
                {
                    Id = (int)permission,
                    Name = permission.ToString()
                });
            }
        }

        await context.SaveChangesAsync();
    }
}