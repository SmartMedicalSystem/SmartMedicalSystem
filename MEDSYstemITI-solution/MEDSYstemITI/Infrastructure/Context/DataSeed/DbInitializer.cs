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

        // ── Apply any pending migrations ─────────────────────────────────────────
        await context.Database.MigrateAsync();

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
