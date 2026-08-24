using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class SessionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Sessions.AnyAsync())
            return;

        static string Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : string.Join(" ", value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .ToLowerInvariant();

        var patients = await context.Patients.AsNoTracking().ToListAsync();
        var patientIds = patients
            .GroupBy(p => Normalize($"{p.FirstName} {p.LastName}"))
            .ToDictionary(g => g.Key, g => g.First().Id);

        var doctors = await context.Doctors.AsNoTracking().Where(d => d.Email != null).ToListAsync();
        var doctorIds = doctors
            .GroupBy(d => Normalize(d.Email))
            .ToDictionary(g => g.Key, g => g.First().Id);

        var departments = await context.Departments.AsNoTracking().ToListAsync();
        var departmentIds = departments
            .GroupBy(d => Normalize(d.Name))
            .ToDictionary(g => g.Key, g => g.First().Id);

        if (patientIds.Count == 0 || doctorIds.Count == 0 || departmentIds.Count == 0)
            return;

        int PatientId(string name) =>
            patientIds.TryGetValue(Normalize(name), out var id)
                ? id
                : throw new InvalidOperationException($"Session seed patient '{name}' was not found.");

        int DoctorId(string email) =>
            doctorIds.TryGetValue(Normalize(email), out var id)
                ? id
                : throw new InvalidOperationException($"Session seed doctor '{email}' was not found.");

        int DepartmentId(string name) =>
            departmentIds.TryGetValue(Normalize(name), out var id)
                ? id
                : throw new InvalidOperationException($"Session seed department '{name}' was not found.");

        var sessions = new List<Session>
        {
            Create(PatientId("Mohamed Saied"), DoctorId("mohamed48289@gmail.com"), DepartmentId("Cardiology"),
                new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                "Patient presents with chest tightness. ECG ordered."),

            Create(PatientId("yara mohamed"), DoctorId("mohamed48289@gmail.com"), DepartmentId("Cardiology"),
                new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                "Follow-up for hypertension management."),

            Create(PatientId("mohamed hassan"), DoctorId("mohamedsaiedhassan308@gmail.com"), DepartmentId("Neurology"),
                new DateTime(2024, 2, 5, 10, 30, 0, DateTimeKind.Utc),
                "Migraine with aura. MRI of the brain recommended."),

            // ⚠️ كانت "Nour Ahmed" - المريض ده مش موجود خالص في PatientSeeder الحالي.
            // استبدلتها بـ "Yara Mohamed" (المريض التاني اللي اسمه Yara بحرف Y كابيتال،
            // كان الوحيد من الـ 12 مريض اللي مش متستخدم في أي session تحت).
            Create(PatientId("Yara Mohamed"), DoctorId("mohamedsaiedhassan308@gmail.com"), DepartmentId("Neurology"),
                new DateTime(2024, 2, 12, 14, 0, 0, DateTimeKind.Utc),
                "Peripheral neuropathy work-up. Blood glucose and B12 ordered."),

            Create(PatientId("Karim Samir"), DoctorId("khaled.ali@medsystem.local"), DepartmentId("Orthopedics"),
                new DateTime(2024, 2, 20, 8, 0, 0, DateTimeKind.Utc),
                "Knee pain. X-ray shows mild osteoarthritis."),

            Create(PatientId("Yasmine Khalil"), DoctorId("khaled.ali@medsystem.local"), DepartmentId("Orthopedics"),
                new DateTime(2024, 3, 1, 9, 30, 0, DateTimeKind.Utc),
                "Post-operative check after hip replacement."),

            Create(PatientId("Hossam Maher"), DoctorId("mona.ibrahim@medsystem.local"), DepartmentId("Pediatrics"),
                new DateTime(2024, 3, 8, 10, 0, 0, DateTimeKind.Utc),
                "Routine growth assessment. Vaccination updated."),

            Create(PatientId("Salma Tarek"), DoctorId("salwasayed522@gmail.com"), DepartmentId("Internal Medicine"),
                new DateTime(2024, 3, 14, 13, 0, 0, DateTimeKind.Utc),
                "Type 2 Diabetes follow-up. HbA1c and lipid profile ordered."),

            Create(PatientId("Omar Farouk"), DoctorId("salwasayed522@gmail.com"), DepartmentId("Internal Medicine"),
                new DateTime(2024, 3, 21, 15, 0, 0, DateTimeKind.Utc),
                "Fatigue and weight loss. CBC and thyroid panel ordered."),

            Create(PatientId("Dina Mustafa"), DoctorId("salwasayed522@gmail.com"), DepartmentId("Internal Medicine"),
                new DateTime(2024, 4, 2, 9, 0, 0, DateTimeKind.Utc),
                "Hypertension assessment. Electrolytes and KFT ordered."),

            Create(PatientId("Mahmoud Refaat"), DoctorId("yaraamohamedds232@gmail.com"), DepartmentId("Dermatology"),
                new DateTime(2024, 4, 10, 11, 30, 0, DateTimeKind.Utc),
                "Chronic urticaria. LFT and CBC for baseline."),

            Create(PatientId("Rana Gamal"), DoctorId("tarek.mostafa@medsystem.local"), DepartmentId("General Surgery"),
                new DateTime(2024, 4, 18, 8, 30, 0, DateTimeKind.Utc),
                "Pre-operative assessment for laparoscopic cholecystectomy.")
        };

        await context.Sessions.AddRangeAsync(sessions);
        await context.SaveChangesAsync();
    }

    private static Session Create(int patientId, int doctorId, int deptId, DateTime date, string? notes = null) =>
        new Session(patientId, doctorId, deptId, date, notes)
        {
            CreatedAt = DateTime.UtcNow
        };
}