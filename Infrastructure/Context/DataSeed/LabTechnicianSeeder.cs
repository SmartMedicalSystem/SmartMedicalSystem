using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class LabTechnicianSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var laboratoryIds =
            await context.Laboratories
                .Where(l => l.Code != null)
                .ToDictionaryAsync(
                    l => l.Code!,
                    l => l.Id);

        if (laboratoryIds.Count == 0)
            return;

        int LaboratoryId(string code)
        {
            return laboratoryIds[code];
        }

        var technicians = new List<LabTechnician>
        {
            CreateTechnician(
                "Amir",
                "Hassan",
                "01112345678",
                "mohamed48289@gmail.com",
                "tech.amir.hassan",
                LaboratoryId("LAB-HEMA"),
                "Senior Lab Technician",
                EmploymentStatus.FullTime,
                WorkShift.Morning,
                new DateOnly(2016, 4, 1),
                9,
                Gender.Male,
                new DateTime(1988, 5, 12),
                "Cairo"),

            CreateTechnician(
                "lotfy",
                "khattab",
                "01123456789",
                "lotfykhattab95@gmail.com",
                "tech.lotfy.khattab",
                LaboratoryId("LAB-CHEM"),
                "Lab Technician",
                EmploymentStatus.FullTime,
                WorkShift.Morning,
                new DateOnly(2019, 9, 15),
                6,
                Gender.Male,
                new DateTime(1992, 2, 20),
                "Giza"),

            CreateTechnician(
                "akram",
                "muhammad",
                "01134567890",
                "akramuhammad95@gmail.com",
                "tech.akram.muhammad",
                LaboratoryId("LAB-MICRO"),
                "Lab Technician",
                EmploymentStatus.FullTime,
                WorkShift.Evening,
                new DateOnly(2020, 1, 10),
                5,
                Gender.Male,
                new DateTime(1994, 7, 3),
                "Alexandria"),

            CreateTechnician(
                "Hana",
                "Wael",
                "01145678901",
                "hana.wael@medsystem.local",
                "tech.hana.wael",
                LaboratoryId("LAB-PATH"),
                "Lab Technician",
                EmploymentStatus.PartTime,
                WorkShift.Evening,
                new DateOnly(2021, 6, 1),
                4,
                Gender.Female,
                new DateTime(1996, 11, 8),
                "Cairo"),

            CreateTechnician(
                "Ramy",
                "El-Sayed",
                "01156789012",
                "ramy.elsayed@medsystem.local",
                "tech.ramy.elsayed",
                LaboratoryId("LAB-MICRO"),
                "Chief Lab Technician",
                EmploymentStatus.FullTime,
                WorkShift.Morning,
                new DateOnly(2012, 3, 20),
                13,
                Gender.Male,
                new DateTime(1983, 9, 27),
                "Mansoura"),

            CreateTechnician(
                "Dalia",
                "Sherif",
                "01167890123",
                "dalia.sherif@medsystem.local",
                "tech.dalia.sherif",
                LaboratoryId("LAB-PATH"),
                "Lab Technician",
                EmploymentStatus.FullTime,
                WorkShift.Night,
                new DateOnly(2018, 8, 5),
                7,
                Gender.Female,
                new DateTime(1991, 4, 16),
                "Giza"),

            CreateTechnician(
                "Tarek",
                "Amin",
                "01178901234",
                "tarek.amin@medsystem.local",
                "tech.tarek.amin",
                LaboratoryId("LAB-PATH"),
                "Senior Lab Technician",
                EmploymentStatus.FullTime,
                WorkShift.Night,
                new DateOnly(2015, 11, 1),
                10,
                Gender.Male,
                new DateTime(1987, 1, 30),
                "Cairo"),

            CreateTechnician(
                "Menna",
                "Gamal",
                "01189012345",
                "menna.gamal@medsystem.local",
                "tech.menna.gamal",
                LaboratoryId("LAB-PATH"),
                "Lab Technician",
                EmploymentStatus.Contract,
                WorkShift.Morning,
                new DateOnly(2022, 2, 14),
                3,
                Gender.Female,
                new DateTime(1998, 6, 22),
                "Aswan")
        };

        foreach (var technician in technicians)
        {
            var existingTechnician =
                await context.LabTechnicians
                    .FirstOrDefaultAsync(
                        t => t.Email == technician.Email);

            if (existingTechnician is null)
            {
                await context.LabTechnicians.AddAsync(
                    technician);
            }
        }

        await context.SaveChangesAsync();

        var savedTechnicians =
            await context.LabTechnicians
                .ToListAsync();

        foreach (var technician in savedTechnicians)
        {
            var existingUser =
                await userManager.FindByEmailAsync(
                    technician.Email);

            if (existingUser is not null)
            {
                if (existingUser.PersonId != technician.Id)
                {
                    existingUser.PersonId =
                        technician.Id;

                    await userManager.UpdateAsync(
                        existingUser);
                }

                continue;
            }

            var user = new ApplicationUser
            {
                UserName = technician.Username,

                Email = technician.Email,

                FullName = technician.FullName,

                EmailConfirmed = true,

                PersonId = technician.Id,

                AllowLogin = true,

                AccountActive = true,

                ReceiveNotifications = true
            };

            var result =
                await userManager.CreateAsync(
                    user,
                    "LabTech@12345");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to create lab technician user " +
                    $"{technician.Email}: {errors}");
            }

            await userManager.AddToRoleAsync(
                user,
                Roles.LabTechnician.ToString());
        }
    }

    private static LabTechnician CreateTechnician(
        string firstName,
        string lastName,
        string phone,
        string email,
        string username,
        int laboratoryId,
        string jobTitle,
        EmploymentStatus employmentStatus,
        WorkShift workShift,
        DateOnly joiningDate,
        int yearsOfExperience,
        Gender gender,
        DateTime dateOfBirth,
        string city)
    {
        return new LabTechnician(
            $"{firstName} {lastName}",
            phone)
        {
            FirstName = firstName,

            LastName = lastName,

            Email = email,

            Username = username,

            PhoneNumber = phone,

            LaboratoryId = laboratoryId,

            JobTitle = jobTitle,

            EmploymentStatus = employmentStatus,

            WorkShift = workShift,

            JoiningDate = joiningDate,

            YearsOfExperience = yearsOfExperience,

            Gender = gender,

            DateOfBirth = dateOfBirth,

            Nationality = "Egyptian",

            EncryptedNationalId =
                Guid.NewGuid().ToString("N"),

            Address =
                $"{city}, Egypt",

            City = city,

            Country = "Egypt",

            AllowLogin = true,

            AccountActive = true,

            ReceiveNotifications = true,

            CreatedAt = DateTime.UtcNow
        };
    }
}