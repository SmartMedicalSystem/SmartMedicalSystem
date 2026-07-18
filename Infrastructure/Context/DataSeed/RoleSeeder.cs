using Domain.Enums;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataSeed;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
    {
        string[] roles =
        {
            Roles.Admin.ToString(),
            Roles.Doctor.ToString(),
            Roles.DepartmentManager.ToString(),
            Roles.LabTechnician.ToString()
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role
                });
            }
        }
    }
}