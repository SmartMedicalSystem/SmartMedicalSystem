using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds LabTechnician entity rows AND creates the matching ApplicationUser login accounts
/// with the LabTechnician role. No FK dependency on other domain entities.
/// </summary>
public static class LabTechnicianSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.LabTechnicians.AnyAsync())
            return;

        // ── 1. Seed domain LabTechnician records ────────────────────────────────
        var techs = new List<LabTechnician>
        {
            CreateTech( "Amir Hassan",     "01112345678"),
            CreateTech( "Samira Nour",      "01123456789"),
            CreateTech( "Bassem Fouad",     "01134567890"),
            CreateTech( "Hana Wael",        "01145678901"),
            CreateTech( "Ramy El-Sayed",    "01156789012"),
            CreateTech( "Dalia Sherif",     "01167890123"),
            CreateTech( "Tarek Amin",       "01178901234"),
            CreateTech( "Menna Gamal",      "01189012345"),
        };

        await context.LabTechnicians.AddRangeAsync(techs);
        await context.SaveChangesAsync();

        // ── 2. Create Identity login accounts for each lab technician ───────────
        var credentials = new[]
        {
            (email: "amir.hassan@medsystem.local",    username: "tech.amir.hassan",    name: "Amir Hassan"),
            (email: "samira.nour@medsystem.local",    username: "tech.samira.nour",    name: "Samira Nour"),
            (email: "bassem.fouad@medsystem.local",   username: "tech.bassem.fouad",   name: "Bassem Fouad"),
            (email: "hana.wael@medsystem.local",      username: "tech.hana.wael",      name: "Hana Wael"),
            (email: "ramy.elsayed@medsystem.local",   username: "tech.ramy.elsayed",   name: "Ramy El-Sayed"),
            (email: "dalia.sherif@medsystem.local",   username: "tech.dalia.sherif",   name: "Dalia Sherif"),
            (email: "tarek.amin@medsystem.local",     username: "tech.tarek.amin",     name: "Tarek Amin"),
            (email: "menna.gamal@medsystem.local",    username: "tech.menna.gamal",    name: "Menna Gamal"),
        };

        foreach (var (email, username, name) in credentials)
        {
            if (await userManager.FindByEmailAsync(email) is not null)
                continue;

            var user = new ApplicationUser
            {
                UserName       = username,
                Email          = email,
                FullName       = name,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "LabTech@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, Roles.LabTechnician.ToString());
        }
    }

    // ── Helper ──────────────────────────────────────────────────────────────────
    private static LabTechnician CreateTech(string name, string contact) =>
        new LabTechnician(name, contact)
        {
           
            CreatedAt = DateTime.UtcNow
        };
}
