using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds Patient entity rows AND creates the matching ApplicationUser login accounts.
/// No FK dependencies on other domain tables – can run in any order after roles exist.
/// </summary>
public static class PatientSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.Patients.AnyAsync())
            return;

        // ── 1. Seed domain Patient records ──────────────────────────────────────
        var patients = new List<Patient>
        {
            CreatePatient( "Mohamed",  "Youssef",  20100001, new DateTime(1990, 3, 12), Gender.Male,   1012345678, "15 Heliopolis St., Cairo",       BloodType.APositive),
            CreatePatient( "Fatima",   "Hassan",   20200002, new DateTime(1985, 7, 24), Gender.Female, 1023456789, "32 Mohandiseen Blvd., Giza",     BloodType.BNegative),
            CreatePatient( "Ali",      "Ibrahim",  20300003, new DateTime(2000, 1, 5),  Gender.Male,   1034567890, "7 El-Maadi, Cairo",              BloodType.OPositive),
            CreatePatient( "Nour",     "Ahmed",    20400004, new DateTime(1978, 11, 17),Gender.Female, 1045678901, "20 Smouha, Alexandria",          BloodType.ABPositive),
            CreatePatient( "Karim",    "Samir",    20500005, new DateTime(1995, 5, 30), Gender.Male,   1056789012, "3 Zamalek, Cairo",               BloodType.ANegative),
            CreatePatient( "Yasmine",  "Khalil",   20600006, new DateTime(1982, 9, 8),  Gender.Female, 1067890123, "44 El-Rehab City, Cairo",        BloodType.BPositive),
            CreatePatient( "Hossam",   "Maher",    20700007, new DateTime(1970, 4, 22), Gender.Male,   1078901234, "11 Nasr City, Cairo",            BloodType.ONegative),
            CreatePatient( "Salma",    "Tarek",    20800008, new DateTime(2005, 6, 14), Gender.Female, 1089012345, "5 6th of October City, Giza",   BloodType.ABNegative),
            CreatePatient( "Omar",     "Farouk",   20900009, new DateTime(1965, 12, 3), Gender.Male,   1090123456, "18 Aswan St., Aswan",            BloodType.APositive),
            CreatePatient( "Dina",     "Mustafa",  21000010, new DateTime(1993, 2, 28), Gender.Female, 1001234567, "26 Mansoura Rd., Mansoura",      BloodType.OPositive),
            CreatePatient( "Mahmoud",  "Refaat",   21100011, new DateTime(1988, 8, 19), Gender.Male,   1112345678, "9 Sohag St., Sohag",             BloodType.BPositive),
            CreatePatient( "Rana",     "Gamal",    21200012, new DateTime(1997, 10, 7), Gender.Female, 1123456789, "37 Luxor Rd., Luxor",            BloodType.ABPositive),
        };

        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();

        // ── 2. Create Identity login accounts (no role – patients aren't system users) ──
        // If your system has a Patient role, uncomment the AddToRoleAsync call below.
        var credentials = new[]
        {
            (email: "mohamed.youssef@medsystem.local",  username: "mohamed.youssef",  name: "Mohamed Youssef"),
            (email: "fatima.hassan@medsystem.local",    username: "fatima.hassan",    name: "Fatima Hassan"),
            (email: "ali.ibrahim@medsystem.local",      username: "ali.ibrahim",      name: "Ali Ibrahim"),
            (email: "nour.ahmed@medsystem.local",       username: "nour.ahmed",       name: "Nour Ahmed"),
            (email: "karim.samir@medsystem.local",      username: "karim.samir",      name: "Karim Samir"),
            (email: "yasmine.khalil@medsystem.local",   username: "yasmine.khalil",   name: "Yasmine Khalil"),
            (email: "hossam.maher@medsystem.local",     username: "hossam.maher",     name: "Hossam Maher"),
            (email: "salma.tarek@medsystem.local",      username: "salma.tarek",      name: "Salma Tarek"),
            (email: "omar.farouk@medsystem.local",      username: "omar.farouk",      name: "Omar Farouk"),
            (email: "dina.mustafa@medsystem.local",     username: "dina.mustafa",     name: "Dina Mustafa"),
            (email: "mahmoud.refaat@medsystem.local",   username: "mahmoud.refaat",   name: "Mahmoud Refaat"),
            (email: "rana.gamal@medsystem.local",       username: "rana.gamal",       name: "Rana Gamal"),
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

            await userManager.CreateAsync(user, "Patient@12345");
            // Uncomment if a Patient role is added in the future:
            // if (result.Succeeded)
            //     await userManager.AddToRoleAsync(user, Roles.Patient.ToString());
        }
    }

    // ── Helper ──────────────────────────────────────────────────────────────────
    private static Patient CreatePatient(
        string firstName, string lastName, int nationalId,
        DateTime dob, Gender gender, int mobile, string address, BloodType blood)
    {
        var p = new Patient(firstName, lastName, nationalId, dob, gender, mobile, address, blood)
        {
           
            CreatedAt = DateTime.UtcNow
        };
        return p;
    }
}
