using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds Doctor entity rows AND creates the matching ApplicationUser login accounts
/// (role: Doctor). Departments must already exist before this runs.
/// </summary>
public static class DoctorSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.Doctors.AnyAsync())
            return;

        // ── 1. Seed domain Doctor records ──────────────────────────────────────
        var doctors = new List<Doctor>
        {
            //CreateDoctor(1,  "Dr. Ahmed Hassan",    "Cardiology",         "01001234567", Gender.Male,   1,  new DateTime(1975, 3, 15), "ahmed.hassan@medsystem.local",    "12 Tahrir Square, Cairo"),
            //CreateDoctor(2,  "Dr. Sara Mohamed",    "Neurology",           "01012345678", Gender.Female, 2,  new DateTime(1980, 7, 22), "sara.mohamed@medsystem.local",    "45 Nile St., Giza"),
            //CreateDoctor(3,  "Dr. Khaled Ali",      "Orthopedics",         "01023456789", Gender.Male,   3,  new DateTime(1972, 11, 5), "khaled.ali@medsystem.local",      "8 Al-Haram Blvd., Giza"),
            //CreateDoctor(4,  "Dr. Mona Ibrahim",    "Pediatrics",          "01034567890", Gender.Female, 4,  new DateTime(1985, 1, 30), "mona.ibrahim@medsystem.local",    "22 Port Said St., Alexandria"),
            //CreateDoctor(5,  "Dr. Youssef Nasser",  "Oncology",            "01045678901", Gender.Male,   5,  new DateTime(1968, 9, 14), "youssef.nasser@medsystem.local",  "7 El-Nasr Rd., Cairo"),
            //CreateDoctor(6,  "Dr. Nadia Farouk",    "Radiology",           "01056789012", Gender.Female, 6,  new DateTime(1978, 4, 18), "nadia.farouk@medsystem.local",    "33 October 6th St., Giza"),
            //CreateDoctor(7,  "Dr. Tarek Mostafa",   "General Surgery",     "01067890123", Gender.Male,   7,  new DateTime(1970, 6, 25), "tarek.mostafa@medsystem.local",   "17 Salah Salem St., Cairo"),
            //CreateDoctor(8,  "Dr. Heba Salah",      "Internal Medicine",   "01078901234", Gender.Female, 8,  new DateTime(1982, 12, 8), "heba.salah@medsystem.local",      "5 Corniche El-Nil, Cairo"),
            //CreateDoctor(9,  "Dr. Omar Zaki",       "Dermatology",         "01089012345", Gender.Male,   9,  new DateTime(1988, 2, 19), "omar.zaki@medsystem.local",       "29 Ahmed Urabi St., Cairo"),
            //CreateDoctor(10, "Dr. Rania Adel",      "Laboratory Medicine", "01090123456", Gender.Female, 10, new DateTime(1983, 8, 11), "rania.adel@medsystem.local",      "14 El-Galaa St., Mansoura"),
            CreateDoctor( "Dr. Ahmed Hassan",    "Cardiology",         "01001234567", Gender.Male,   1,  new DateTime(1975, 3, 15), "ahmed.hassan@medsystem.local",    "12 Tahrir Square, Cairo"),
            CreateDoctor( "Dr. Sara Mohamed",    "Neurology",           "01012345678", Gender.Female, 2,  new DateTime(1980, 7, 22), "sara.mohamed@medsystem.local",    "45 Nile St., Giza"),
            CreateDoctor( "Dr. Khaled Ali",      "Orthopedics",         "01023456789", Gender.Male,   3,  new DateTime(1972, 11, 5), "khaled.ali@medsystem.local",      "8 Al-Haram Blvd., Giza"),
            CreateDoctor( "Dr. Mona Ibrahim",    "Pediatrics",          "01034567890", Gender.Female, 4,  new DateTime(1985, 1, 30), "mona.ibrahim@medsystem.local",    "22 Port Said St., Alexandria"),
            CreateDoctor( "Dr. Youssef Nasser",  "Oncology",            "01045678901", Gender.Male,   5,  new DateTime(1968, 9, 14), "youssef.nasser@medsystem.local",  "7 El-Nasr Rd., Cairo"),
            CreateDoctor( "Dr. Nadia Farouk",    "Radiology",           "01056789012", Gender.Female, 6,  new DateTime(1978, 4, 18), "nadia.farouk@medsystem.local",    "33 October 6th St., Giza"),
            CreateDoctor( "Dr. Tarek Mostafa",   "General Surgery",     "01067890123", Gender.Male,   7,  new DateTime(1970, 6, 25), "tarek.mostafa@medsystem.local",   "17 Salah Salem St., Cairo"),
            CreateDoctor( "Dr. Heba Salah",      "Internal Medicine",   "01078901234", Gender.Female, 8,  new DateTime(1982, 12, 8), "heba.salah@medsystem.local",      "5 Corniche El-Nil, Cairo"),
            CreateDoctor( "Dr. Omar Zaki",       "Dermatology",         "01089012345", Gender.Male,   9,  new DateTime(1988, 2, 19), "omar.zaki@medsystem.local",       "29 Ahmed Urabi St., Cairo"),
            CreateDoctor( "Dr. Rania Adel",      "Laboratory Medicine", "01090123456", Gender.Female, 10, new DateTime(1983, 8, 11), "rania.adel@medsystem.local",      "14 El-Galaa St., Mansoura"),

        };

        await context.Doctors.AddRangeAsync(doctors);
        await context.SaveChangesAsync();

        // ── 2. Create Identity login accounts for each doctor ─────────────────
        var credentials = new[]
        {
            (email: "ahmed.hassan@medsystem.local",   username: "dr.ahmed.hassan",   name: "Dr. Ahmed Hassan"),
            (email: "sara.mohamed@medsystem.local",    username: "dr.sara.mohamed",    name: "Dr. Sara Mohamed"),
            (email: "khaled.ali@medsystem.local",      username: "dr.khaled.ali",      name: "Dr. Khaled Ali"),
            (email: "mona.ibrahim@medsystem.local",    username: "dr.mona.ibrahim",    name: "Dr. Mona Ibrahim"),
            (email: "youssef.nasser@medsystem.local",  username: "dr.youssef.nasser",  name: "Dr. Youssef Nasser"),
            (email: "nadia.farouk@medsystem.local",    username: "dr.nadia.farouk",    name: "Dr. Nadia Farouk"),
            (email: "tarek.mostafa@medsystem.local",   username: "dr.tarek.mostafa",   name: "Dr. Tarek Mostafa"),
            (email: "heba.salah@medsystem.local",      username: "dr.heba.salah",      name: "Dr. Heba Salah"),
            (email: "omar.zaki@medsystem.local",       username: "dr.omar.zaki",       name: "Dr. Omar Zaki"),
            (email: "rania.adel@medsystem.local",      username: "dr.rania.adel",      name: "Dr. Rania Adel"),
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

            var result = await userManager.CreateAsync(user, "Doctor@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, Roles.Doctor.ToString());
        }
    }

    // ── Helper ──────────────────────────────────────────────────────────────────
    // No-op placeholder to trigger patch apply
    private static Doctor CreateDoctor(
         string name, string specialization, string contact,
        Gender gender, int departmentId, DateTime dob, string email, string address)
    {
        // Strip the leading '0' so the phone string fits in an int (e.g. "01012345678" → 1012345678)
        var mobileDigits = contact.TrimStart('0');
        var d = new Doctor(name, specialization, contact, gender, departmentId)
        {
            Email = email,
            PhoneNumber = contact,
            Address = address,
            DateOfBirth = dob,
            EncryptedNationalId = (10000000 + new Random().Next(1, 1000000)).ToString(),   // synthetic unique National ID (plain for seed)
            CreatedAt = DateTime.UtcNow
        };
        return d;
    }
}
