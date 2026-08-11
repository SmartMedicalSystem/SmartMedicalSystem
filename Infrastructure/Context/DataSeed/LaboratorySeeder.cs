using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class LaboratorySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Laboratories.AnyAsync())
            return;

        var departmentIds = await context.Departments
            .ToDictionaryAsync(d => d.Name, d => d.Id);

        if (!departmentIds.TryGetValue("Laboratory", out var laboratoryDepartmentId))
            return;

        var laboratories = new List<Laboratory>
        {
            Create("Central Chemistry Laboratory", "First Floor - Laboratory Wing", "01011112222", LabStatus.Active,
                laboratoryDepartmentId, "LAB-CHEM", "Clinical Chemistry"),

            Create("Hematology Laboratory", "First Floor - Laboratory Wing", "01111112222", LabStatus.Active,
                laboratoryDepartmentId, "LAB-HEMA", "Hematology"),

            Create("Microbiology Laboratory", "Second Floor - Laboratory Wing", "01211112222", LabStatus.Active,
                laboratoryDepartmentId, "LAB-MICRO", "Microbiology"),

            Create("Pathology Laboratory", "Second Floor - Laboratory Wing", "01511112222", LabStatus.Active,
                laboratoryDepartmentId, "LAB-PATH", "Pathology")
        };

        await context.Laboratories.AddRangeAsync(laboratories);
        await context.SaveChangesAsync();
    }

    private static Laboratory Create(
        string name,
        string location,
        string phone,
        LabStatus status,
        int departmentId,
        string code,
        string specialty) =>
        new Laboratory(name, location, phone, status, departmentId: departmentId, code: code, specialty: specialty)
        {
            CreatedAt = DateTime.UtcNow
        };
}
