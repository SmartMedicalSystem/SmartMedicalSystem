using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class DoctorSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
    {
        var departmentList = await context.Departments
            .Select(d => new { d.Name, d.Id })
            .ToListAsync();

        var departmentIds = departmentList
            .ToDictionary(d => (d.Name ?? string.Empty).Trim(), d => d.Id, StringComparer.OrdinalIgnoreCase);

        if (departmentIds.Count == 0)
            return;

        int DepartmentId(string name)
        {
            var key = (name ?? string.Empty).Trim();
            if (!departmentIds.TryGetValue(key, out var id))
            {
                // Try tolerant matches: starts-with or contains (case-insensitive)
                var tolerant = departmentIds.Keys
                    .FirstOrDefault(k => k.StartsWith(key, StringComparison.OrdinalIgnoreCase)
                                         || k.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0);

                if (tolerant != null && departmentIds.TryGetValue(tolerant, out var tolerantId))
                {
                    return tolerantId;
                }

                throw new InvalidOperationException($"Department '{name}' not found. Available: {string.Join(", ", departmentIds.Keys)}");
            }

            return id;
        }

        var doctors = new List<Doctor>
        {
            CreateDoctor(
                "Ahmed",
                "Hassan",
                "Cardiology",
                "01001234567",
                Gender.Male,
                DepartmentId("Cardiology"),
                new DateTime(1975, 3, 15),
                "ahmed.hassan@medsystem.local",
                "12 Tahrir Square",
                "Cairo"),


            CreateDoctor(
                "Sara",
                "Mohamed",
                "Neurology",
                "01012345678",
                Gender.Female,
                DepartmentId("Neurology"),
                new DateTime(1980, 7, 22),
                "sara.mohamed@medsystem.local",
                "45 Nile Street",
                "Giza"),

            CreateDoctor(
                "Khaled",
                "Ali",
                "Orthopedics",
                "01023456789",
                Gender.Male,
                DepartmentId("Orthopedics"),
                new DateTime(1972, 11, 5),
                "khaled.ali@medsystem.local",
                "8 Al-Haram Boulevard",
                "Giza"),

            CreateDoctor(
                "Mona",
                "Ibrahim",
                "Pediatrics",
                "01034567890",
                Gender.Female,
                DepartmentId("Pediatrics"),
                new DateTime(1985, 1, 30),
                "mona.ibrahim@medsystem.local",
                "22 Port Said Street",
                "Alexandria"),

            CreateDoctor(
                "Youssef",
                "Nasser",
                "Oncology",
                "01045678901",
                Gender.Male,
                DepartmentId("Oncology"),
                new DateTime(1968, 9, 14),
                "youssef.nasser@medsystem.local",
                "7 El-Nasr Road",
                "Cairo"),

            CreateDoctor(
                "Nadia",
                "Farouk",
                "Radiology",
                "01056789012",
                Gender.Female,
                DepartmentId("Radiology"),
                new DateTime(1978, 4, 18),
                "nadia.farouk@medsystem.local",
                "33 October 6th Street",
                "Giza"),

            CreateDoctor(
                "Tarek",
                "Mostafa",
                "General Surgery",
                "01067890123",
                Gender.Male,
                DepartmentId("General Surgery"),
                new DateTime(1970, 6, 25),
                "tarek.mostafa@medsystem.local",
                "17 Salah Salem Street",
                "Cairo"),

            CreateDoctor(
                "Heba",
                "Salah",
                "Internal Medicine",
                "01078901234",
                Gender.Female,
                DepartmentId("Internal Medicine"),
                new DateTime(1982, 12, 8),
                "heba.salah@medsystem.local",
                "5 Corniche El-Nil",
                "Cairo"),

            CreateDoctor(
                "Omar",
                "Zaki",
                "Dermatology",
                "01089012345",
                Gender.Male,
                DepartmentId("Dermatology"),
                new DateTime(1988, 2, 19),
                "omar.zaki@medsystem.local",
                "29 Ahmed Urabi Street",
                "Cairo"),

            CreateDoctor(
                "Rania",
                "Adel",
                "Laboratory Medicine",
                "01090123456",
                Gender.Female,
                DepartmentId("Laboratory"),
                new DateTime(1983, 8, 11),
                "rania.adel@medsystem.local",
                "14 El-Galaa Street",
                "Mansoura",
                "/uploads/04a2fe3e-0440-4e51-99f1-dce59d2b59fd.jpg"
                )
        };

        foreach (var doctor in doctors)
        {
            var existingDoctor =
                await context.Doctors
                    .FirstOrDefaultAsync(
                        d => d.Email == doctor.Email);

            if (existingDoctor is null)
            {
                await context.Doctors.AddAsync(doctor);
            }
        }

        await context.SaveChangesAsync();

        var savedDoctors =
            await context.Doctors
                .ToListAsync();

        foreach (var doctor in savedDoctors)
        {
            var existingUser =
                await userManager.FindByEmailAsync(
                    doctor.Email);

            if (existingUser is not null)
            {
                if (existingUser.PersonId != doctor.Id)
                {
                    existingUser.PersonId = doctor.Id;

                    await userManager.UpdateAsync(
                        existingUser);
                }

                continue;
            }

            var username =
                $"dr.{doctor.FirstName.ToLowerInvariant()}." +
                $"{doctor.LastName.ToLowerInvariant()}";

            var user = new ApplicationUser
            {
                UserName = username,

                Email = doctor.Email,

                FullName =
                    $"{doctor.FirstName} {doctor.LastName}",

                EmailConfirmed = true,

                PersonId = doctor.Id,

                AllowLogin = true,
                AccountActive = true,
                ReceiveNotifications = true
            };

            var result =
                await userManager.CreateAsync(
                    user,
                    "Doctor@12345");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to create doctor user " +
                    $"{doctor.Email}: {errors}");
            }

            await userManager.AddToRoleAsync(
                user,
                Roles.Doctor.ToString());
        }
    }

    private static Doctor CreateDoctor(
        string firstName,
        string lastName,
        string specialization,
        string phone,
        Gender gender,
        int departmentId,
        DateTime dateOfBirth,
        string email,
        string address,
        string city,
        string? photoUrl)
    {
        return new Doctor(
            $"{firstName} {lastName}",
            specialization,
            phone,
            gender,
            departmentId)
        {
            FirstName = firstName,
            LastName = lastName,

            Email = email,

            PhoneNumber = phone,

            Address = address,

            City = city,

            Country = "Egypt",

            Nationality = "Egyptian",

            Gender = gender,

            DateOfBirth = dateOfBirth,

            EncryptedNationalId =
                Guid.NewGuid().ToString("N"),

            AllowLogin = true,

            AccountActive = true,

            ReceiveNotifications = true,

            CreatedAt = DateTime.UtcNow,
            PhotoUrl = photoUrl
        };
    }

         private static Doctor CreateDoctor(
        string firstName,
        string lastName,
        string specialization,
        string phone,
        Gender gender,
        int departmentId,
        DateTime dateOfBirth,
        string email,
        string address,
        string city)
    {
        return new Doctor(
            $"{firstName} {lastName}",
            specialization,
            phone,
            gender,
            departmentId)
        {
            FirstName = firstName,
            LastName = lastName,

            Email = email,

            PhoneNumber = phone,

            Address = address,

            City = city,

            Country = "Egypt",

            Nationality = "Egyptian",

            Gender = gender,

            DateOfBirth = dateOfBirth,

            EncryptedNationalId =
                Guid.NewGuid().ToString("N"),

            AllowLogin = true,

            AccountActive = true,

            ReceiveNotifications = true,

            CreatedAt = DateTime.UtcNow
        };
    }
}