using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds the Departments table with realistic hospital departments.
/// Must be seeded before Doctors because Doctor.DepartmentId is a required FK.
/// </summary>
public static class DepartmentSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Departments.AnyAsync())
            return;

        var departments = new List<Department>
       {
           //new Department("Cardiology",          "Dr. Ahmed Hassan")       { Id = 1,  CreatedAt = DateTime.UtcNow },
           //new Department("Neurology",            "Dr. Sara Mohamed")       { Id = 2,  CreatedAt = DateTime.UtcNow },
           //new Department("Orthopedics",          "Dr. Khaled Ali")         { Id = 3,  CreatedAt = DateTime.UtcNow },
           //new Department("Pediatrics",           "Dr. Mona Ibrahim")       { Id = 4,  CreatedAt = DateTime.UtcNow },
           //new Department("Oncology",             "Dr. Youssef Nasser")     { Id = 5,  CreatedAt = DateTime.UtcNow },
           //new Department("Radiology",            "Dr. Nadia Farouk")       { Id = 6,  CreatedAt = DateTime.UtcNow },
           //new Department("General Surgery",      "Dr. Tarek Mostafa")      { Id = 7,  CreatedAt = DateTime.UtcNow },
           //new Department("Internal Medicine",    "Dr. Heba Salah")         { Id = 8,  CreatedAt = DateTime.UtcNow },
           //new Department("Dermatology",          "Dr. Omar Zaki")          { Id = 9,  CreatedAt = DateTime.UtcNow },
           //new Department("Laboratory",           "Dr. Rania Adel")         { Id = 10, CreatedAt = DateTime.UtcNow },
            new Department("Cardiology",          "Dr. Ahmed Hassan")       { CreatedAt = DateTime.UtcNow },
           new Department("Neurology",            "Dr. Sara Mohamed")       { CreatedAt = DateTime.UtcNow },
           new Department("Orthopedics",          "Dr. Khaled Ali")         { CreatedAt = DateTime.UtcNow },
           new Department("Pediatrics",           "Dr. Mona Ibrahim")       { CreatedAt = DateTime.UtcNow },
           new Department("Oncology",             "Dr. Youssef Nasser")     { CreatedAt = DateTime.UtcNow },
           new Department("Radiology",            "Dr. Nadia Farouk")       { CreatedAt = DateTime.UtcNow },
           new Department("General Surgery",      "Dr. Tarek Mostafa")      { CreatedAt = DateTime.UtcNow },
           new Department("Internal Medicine",    "Dr. Heba Salah")         { CreatedAt = DateTime.UtcNow },
           new Department("Dermatology",          "Dr. Omar Zaki")          { CreatedAt = DateTime.UtcNow },
           new Department("Laboratory",           "Dr. Rania Adel")         { CreatedAt = DateTime.UtcNow },
       };

        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();
    }
}
