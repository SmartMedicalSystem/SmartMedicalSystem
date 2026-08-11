using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataSeed;

public static class DbInitializer
{
    public const string DefaultAdminEmail =
        "admin@medsystem.local";

    public const string DefaultAdminPassword =
        "Admin@12345";

    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        var context =
            serviceProvider.GetRequiredService<ApplicationDbContext>();

        var roleManager =
            serviceProvider.GetRequiredService<
                RoleManager<ApplicationRole>>();

        var userManager =
            serviceProvider.GetRequiredService<
                UserManager<ApplicationUser>>();

        var connection =
            context.Database.GetDbConnection();

        await connection.OpenAsync();

        await using var lockCmd =
            connection.CreateCommand();

        lockCmd.CommandText =
            "EXEC sp_getapplock " +
            "@Resource = 'MEDSYstemITI_MigrateAndSeed', " +
            "@LockMode = 'Exclusive', " +
            "@LockOwner = 'Session', " +
            "@LockTimeout = 60000;";

        await lockCmd.ExecuteNonQueryAsync();

        try
        {
            await context.Database.MigrateAsync();

            await RunSeedPipelineAsync(
                context,
                roleManager,
                userManager);
        }
        finally
        {
            await using var unlockCmd =
                connection.CreateCommand();

            unlockCmd.CommandText =
                "EXEC sp_releaseapplock " +
                "@Resource = 'MEDSYstemITI_MigrateAndSeed', " +
                "@LockOwner = 'Session';";

            await unlockCmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task RunSeedPipelineAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        // 1. Roles
        await RoleSeeder.SeedAsync(
            roleManager);

        // 2. Permissions / Role Claims
        await RoleClaimsSeeder.SeedAsync(
            context,
            roleManager);

        // 3. Admin
        await SeedAdminUserAsync(
            context,
            userManager);

        // 4. Departments
        await DepartmentSeeder.SeedAsync(
            context);

        // 5. Doctors
        await DoctorSeeder.SeedAsync(
            context,
            userManager);

        // 6. Patients
        await PatientSeeder.SeedAsync(
            context);

        // 7. Laboratories
        await LaboratorySeeder.SeedAsync(
            context);

        // 8. Lab Technicians
        await LabTechnicianSeeder.SeedAsync(
            context,
            userManager);

        // 9. Test Elements
        await TestElementSeeder.SeedAsync(
            context);

        // 10. Lab Tests
        await LabTestSeeder.SeedAsync(
            context);

        // 11. Sessions
        await SessionSeeder.SeedAsync(
            context);

        // 12. Lab Requests
        await RequestLabsSeeder.SeedAsync(
            context);

        // 13. Patient Results
        await PatientResultSeeder.SeedAsync(
            context);

        //14.Notifications
        await NotificationSeeder.SeedAsync(
            context,
            userManager);
    }

    private static async Task SeedAdminUserAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        var existingUser =
            await userManager.FindByEmailAsync(
                DefaultAdminEmail);

        if (existingUser is not null)
            return;

        var adminPerson = new AdminPerson(
            "System",
            "Administrator",
            new DateTime(1990, 1, 1))
        {
            EncryptedNationalId =
                "ADMIN-SYSTEM-001",

            Gender = Gender.Male,

            Nationality = "Egyptian",

            Email = DefaultAdminEmail,

            PhoneNumber = "01000000000",

            Address = "System",

            City = "Cairo",

            Country = "Egypt",

            AllowLogin = true,

            AccountActive = true,

            ReceiveNotifications = true,

            CreatedAt = DateTime.UtcNow
        };

        await context.Set<AdminPerson>()
            .AddAsync(adminPerson);

        await context.SaveChangesAsync();

        var adminUser = new ApplicationUser
        {
            UserName = "admin",

            Email = DefaultAdminEmail,

            FullName = "System Administrator",

            EmailConfirmed = true,

            PersonId = adminPerson.Id,

            AllowLogin = true,

            AccountActive = true,

            ReceiveNotifications = true,

        };

        var result =
            await userManager.CreateAsync(
                adminUser,
                DefaultAdminPassword);

        if (!result.Succeeded)
        {
            var errors =
                string.Join(
                    ", ",
                    result.Errors.Select(
                        e => e.Description));

            throw new InvalidOperationException(
                $"Failed to create admin user: {errors}");
        }

        var roleResult =
            await userManager.AddToRoleAsync(
                adminUser,
                Roles.Admin.ToString());

        if (!roleResult.Succeeded)
        {
            var errors =
                string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        e => e.Description));

            throw new InvalidOperationException(
                $"Failed to assign Admin role: {errors}");
        }
    }
}