using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class PatientSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if (await context.Patients.AnyAsync())
            return;

        var patients = new List<Patient>
        {
            CreatePatient(
                "Mohamed",
                "Youssef",
                "20100001",
                new DateTime(1990, 3, 12),
                Gender.Male,
                "1012345678",
                "15 Heliopolis St., Cairo",
                BloodType.APositive,
                "mohamed.youssef@medsystem.local"),

            CreatePatient(
                "Fatima",
                "Hassan",
                "20200002",
                new DateTime(1985, 7, 24),
                Gender.Female,
                "1023456789",
                "32 Mohandiseen Blvd., Giza",
                BloodType.BNegative,
                "fatima.hassan@medsystem.local"),

            CreatePatient(
                "Ali",
                "Ibrahim",
                "20300003",
                new DateTime(2000, 1, 5),
                Gender.Male,
                "1034567890",
                "7 El-Maadi, Cairo",
                BloodType.OPositive,
                "ali.ibrahim@medsystem.local"),

            CreatePatient(
                "Nour",
                "Ahmed",
                "20400004",
                new DateTime(1978, 11, 17),
                Gender.Female,
                "1045678901",
                "20 Smouha, Alexandria",
                BloodType.ABPositive,
                "nour.ahmed@medsystem.local"),

            CreatePatient(
                "Karim",
                "Samir",
                "20500005",
                new DateTime(1995, 5, 30),
                Gender.Male,
                "1056789012",
                "3 Zamalek, Cairo",
                BloodType.ANegative,
                "karim.samir@medsystem.local"),

            CreatePatient(
                "Yasmine",
                "Khalil",
                "20600006",
                new DateTime(1982, 9, 8),
                Gender.Female,
                "1067890123"    ,
                "44 El-Rehab City, Cairo",
                BloodType.BPositive,
                "yasmine.khalil@medsystem.local"),

            CreatePatient(
                "Hossam",
                "Maher",
                "20700007",
                new DateTime(1970, 4, 22),
                Gender.Male,
                "1078901234",
                "11 Nasr City, Cairo",
                BloodType.ONegative,
                "hossam.maher@medsystem.local"),

            CreatePatient(
                "Salma",
                "Tarek",
                "20800008"  ,
                new DateTime(2005, 6, 14),
                Gender.Female,
                "1089012345",
                "5 6th of October City, Giza",
                BloodType.ABNegative,
                "salma.tarek@medsystem.local"),

            CreatePatient(
                "Omar",
                "Farouk",
                "20900009",
                new DateTime(1965, 12, 3),
                Gender.Male,
                "1090123456",
                "18 Aswan St., Aswan",
                BloodType.APositive,
                "omar.farouk@medsystem.local"),

            CreatePatient(
                "Dina",
                "Mustafa",
                "21000010",
                new DateTime(1993, 2, 28),
                Gender.Female,
                "1001234567",
                "26 Mansoura Rd., Mansoura",
                BloodType.OPositive,
                "dina.mustafa@medsystem.local"),

            CreatePatient(
                "Mahmoud",
                "Refaat",
                "21100011"  ,
                new DateTime(1988, 8, 19),
                Gender.Male,
                "1112345678",
                "9 Sohag St., Sohag",
                BloodType.BPositive,
                "mahmoud.refaat@medsystem.local"),

            CreatePatient(
                "Rana",
                "Gamal",
                "21200012",
                new DateTime(1997, 10, 7),
                Gender.Female,
                "1123456789",
                "37 Luxor Rd., Luxor",
                BloodType.ABPositive,
                "rana.gamal@medsystem.local")
        };

        await context.Patients.AddRangeAsync(
            patients);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(
                ex.InnerException?.Message);

            throw;
        }
    }

    private static Patient CreatePatient(
        string firstName,
        string lastName,
        string nationalId,
        DateTime dateOfBirth,
        Gender gender,
        string mobile,
        string address,
        BloodType bloodType,
        string email)
    {
        var patient = new Patient(
            firstName,
            lastName,
            nationalId,
            dateOfBirth,
            gender,
            mobile,
            address,
            bloodType)
        {
            Email = email,

            PhoneNumber =
                mobile.ToString(),

            Address = address,

            City = GetCityFromAddress(
                address),

            Country = "Egypt",

            Nationality = "Egyptian",

            AllowLogin = false,

            AccountActive = true,

            ReceiveNotifications = true,

            CreatedAt = DateTime.UtcNow
        };

        return patient;
    }

    private static string GetCityFromAddress(
        string address)
    {
        if (address.Contains(
                "Cairo",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Cairo";
        }

        if (address.Contains(
                "Giza",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Giza";
        }

        if (address.Contains(
                "Alexandria",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Alexandria";
        }

        if (address.Contains(
                "Aswan",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Aswan";
        }

        if (address.Contains(
                "Mansoura",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Mansoura";
        }

        if (address.Contains(
                "Sohag",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Sohag";
        }

        if (address.Contains(
                "Luxor",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Luxor";
        }

        return "Unknown";
    }
}