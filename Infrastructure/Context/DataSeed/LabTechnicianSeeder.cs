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

        var laboratoryIds = await context.Laboratories
            .Where(l => l.Code != null)
            .ToDictionaryAsync(l => l.Code!, l => l.Id);

        if (laboratoryIds.Count == 0)
            return;

        int LaboratoryId(string code) => laboratoryIds[code];

        // ── 1. Seed domain LabTechnician records ────────────────────────────────
        // LabTechnicianConfiguration marks Nationality/EncryptedNationalId/Laboratory/
        // JobTitle/Email/Address/City/Country/Username as required columns, so every
        // technician below is given real values instead of relying on empty defaults.
        var techs = new List<LabTechnician>
        {
            CreateTech("Amir",   "Hassan",   "01112345678", "amir.hassan@medsystem.local",    "tech.amir.hassan",
                LaboratoryId("LAB-HEMA"), "Senior Lab Technician", EmploymentStatus.FullTime, WorkShift.Morning,
                new DateOnly(2016, 4, 1),  9, Gender.Male,   new DateTime(1988, 5, 12), "Cairo",     "Egypt"),

            CreateTech("Samira", "Nour",     "01123456789", "samira.nour@medsystem.local",    "tech.samira.nour",
                LaboratoryId("LAB-CHEM"), "Lab Technician",        EmploymentStatus.FullTime, WorkShift.Morning,
                new DateOnly(2019, 9, 15), 6, Gender.Female, new DateTime(1992, 2, 20), "Giza",      "Egypt"),

            CreateTech("Bassem", "Fouad",    "01134567890", "bassem.fouad@medsystem.local",   "tech.bassem.fouad",
                LaboratoryId("LAB-MICRO"), "Lab Technician",        EmploymentStatus.FullTime, WorkShift.Evening,
                new DateOnly(2020, 1, 10), 5, Gender.Male,   new DateTime(1994, 7, 3),  "Alexandria","Egypt"),

            CreateTech("Hana",   "Wael",     "01145678901", "hana.wael@medsystem.local",      "tech.hana.wael",
                LaboratoryId("LAB-PATH"), "Lab Technician",        EmploymentStatus.PartTime, WorkShift.Evening,
                new DateOnly(2021, 6, 1),  4, Gender.Female, new DateTime(1996, 11, 8), "Cairo",     "Egypt"),

            CreateTech("Ramy",   "El-Sayed", "01156789012", "ramy.elsayed@medsystem.local",   "tech.ramy.elsayed",
                LaboratoryId("LAB-MICRO"), "Chief Lab Technician",  EmploymentStatus.FullTime, WorkShift.Morning,
                new DateOnly(2012, 3, 20), 13, Gender.Male,  new DateTime(1983, 9, 27), "Mansoura",  "Egypt"),

            CreateTech("Dalia",  "Sherif",   "01167890123", "dalia.sherif@medsystem.local",   "tech.dalia.sherif",
                LaboratoryId("LAB-PATH"), "Lab Technician",        EmploymentStatus.FullTime, WorkShift.Night,
                new DateOnly(2018, 8, 5),  7, Gender.Female, new DateTime(1991, 4, 16), "Giza",      "Egypt"),

            CreateTech("Tarek",  "Amin",     "01178901234", "tarek.amin@medsystem.local",     "tech.tarek.amin",
                LaboratoryId("LAB-PATH"), "Senior Lab Technician", EmploymentStatus.FullTime, WorkShift.Night,
                new DateOnly(2015, 11, 1), 10, Gender.Male,  new DateTime(1987, 1, 30), "Cairo",     "Egypt"),

            CreateTech("Menna",  "Gamal",    "01189012345", "menna.gamal@medsystem.local",    "tech.menna.gamal",
                LaboratoryId("LAB-PATH"), "Lab Technician",        EmploymentStatus.Contract, WorkShift.Morning,
                new DateOnly(2022, 2, 14), 3, Gender.Female, new DateTime(1998, 6, 22), "Aswan",     "Egypt"),
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
                UserName = username,
                Email = email,
                FullName = name,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "LabTech@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, Roles.LabTechnician.ToString());
        }
    }

    // ── Helper ──────────────────────────────────────────────────────────────────
    private static LabTechnician CreateTech(
        string firstName, string lastName, string contact, string email, string username,
        int laboratoryId, string jobTitle, EmploymentStatus employmentStatus, WorkShift workShift,
        DateOnly joiningDate, int yearsOfExperience, Gender gender, DateTime dateOfBirth,
        string city, string country)
    {
        var t = new LabTechnician($"{firstName} {lastName}", contact)
        {
            Email = email,
            Username = username,
            LaboratoryId = laboratoryId,
            JobTitle = jobTitle,
            EmploymentStatus = employmentStatus,
            WorkShift = workShift,
            JoiningDate = joiningDate,
            YearsOfExperience = yearsOfExperience,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            Nationality = "Egyptian",
            EncryptedNationalId = (20000000 + new Random().Next(1, 1000000)).ToString(),
            Address = $"{city}, {country}",
            //Address = $"{laboratory}, {city}",
            City = city,
            Country = country,
            AllowLogin = true,
            AccountActive = true,
            ReceiveNotifications = true,
            CreatedAt = DateTime.UtcNow
        };
        return t;
    }
}
