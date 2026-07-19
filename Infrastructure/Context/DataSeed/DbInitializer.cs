using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataSeed;

/// <summary>
/// Entry-point for the entire data-seeding pipeline.
///
/// Seeding ORDER (respects foreign-key dependencies):
///   1. Roles                  – no deps
///   2. Permissions            – no deps
///   3. RolePermissions        – roles + permissions
///   4. Admin user             – roles
///   5. Departments            – no deps
///   6. Doctors                – departments + roles
///   7. Patients               – no deps
///   8. LabTechnicians         – roles
///   9. TestElements           – no deps
///  10. LabTests + LabTestElements – test elements
///  11. Sessions               – patients + doctors + departments
///  12. RequestLabs            – sessions + lab tests
///  13. PatientResults + Elements – patients + sessions + lab tests + test elements + lab techs
///  14. Notifications          – users (admin + doctors)
///
/// Call once at startup, e.g.:
///   using (var scope = app.Services.CreateScope())
///       await DbInitializer.SeedAsync(scope.ServiceProvider);
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

        // ── Serialize migration+seeding across concurrent app instances ──────────
        // If two instances of this app start at (almost) the same time — a second
        // debug session, `dotnet watch` restarting while the old process is still
        // shutting down, multiple replicas in production, etc. — they would
        // otherwise both call MigrateAsync()/SaveChanges() against the same tables
        // at once. SQL Server then has to grant conflicting schema/row locks to two
        // transactions that each want what the other is holding, which is a
        // textbook deadlock (Msg 1205) and can abort the migration entirely.
        // sp_getapplock makes every instance queue up and take turns instead.
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        await using (var lockCmd = connection.CreateCommand())
        {
            lockCmd.CommandText =
                "EXEC sp_getapplock @Resource = 'MEDSYstemITI_MigrateAndSeed', @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = 60000;";
            await lockCmd.ExecuteNonQueryAsync();

            try
            {
                // ── Apply any pending migrations ─────────────────────────────────
                await context.Database.MigrateAsync();

                await RunSeedPipelineAsync(context, roleManager, userManager);
            }
            finally
            {
                await using var unlockCmd = connection.CreateCommand();
                unlockCmd.CommandText =
                    "EXEC sp_releaseapplock @Resource = 'MEDSYstemITI_MigrateAndSeed', @LockOwner = 'Session';";
                await unlockCmd.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task RunSeedPipelineAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {

        // ── 1-3: Identity / permission infrastructure ────────────────────────────
        await RoleSeeder.SeedAsync(roleManager);
        await PermissionSeeder.SeedAsync(context);
        await RolePermissionSeeder.SeedAsync(context);

        // ── 4: Admin account ──────────────────────────────────────────────────────
        await SeedAdminUserAsync(userManager);

        // ── 5: Departments (must come before Doctors) ─────────────────────────────
        await DepartmentSeeder.SeedAsync(context);

        // ── 6: Doctors (need departments + Doctor role) ───────────────────────────
        await DoctorSeeder.SeedAsync(context, userManager);

        // ── 7: Patients ───────────────────────────────────────────────────────────
        await PatientSeeder.SeedAsync(context, userManager);

        // ── 8: Lab Technicians ────────────────────────────────────────────────────
        await LabTechnicianSeeder.SeedAsync(context, userManager);

        // ── 9: Test Elements (building blocks of lab tests) ───────────────────────
        await TestElementSeeder.SeedAsync(context);

        // ── 10: Lab Tests + Lab Test Elements (join) ──────────────────────────────
        await LabTestSeeder.SeedAsync(context);

        // ── 11: Sessions (patient × doctor × department) ─────────────────────────
        await SessionSeeder.SeedAsync(context);

        // ── 12: Lab Requests (ordered by a doctor in a session) ───────────────────
        await RequestLabsSeeder.SeedAsync(context);

        // ── 13: Patient Results + Result Elements ─────────────────────────────────
        await PatientResultSeeder.SeedAsync(context);

        // ── 14: Notifications ─────────────────────────────────────────────────────
        await NotificationSeeder.SeedAsync(context, userManager);
    }

    // ── Private helpers ──────────────────────────────────────────────────────────

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(DefaultAdminEmail) is not null)
            return;

        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = DefaultAdminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true
        };

        // NOTE: Move the password to configuration / user-secrets before production deployment.
        var result = await userManager.CreateAsync(adminUser, DefaultAdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
    }
}
