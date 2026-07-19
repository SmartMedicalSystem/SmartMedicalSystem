using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds Session rows that pair Patients with Doctors inside Departments.
/// Requires Patients, Doctors, and Departments to already exist.
/// </summary>
public static class SessionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Sessions.AnyAsync())
            return;

        var sessions = new List<Session>
       {
           // ── Cardiology sessions (DeptId = 1, DoctorId = 1) ──────────────────
      Create(  patientId: 11,  doctorId: 1,  deptId: 1,
       date: new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc),
       notes: "Patient presents with chest tightness. ECG ordered."),

Create(  patientId: 12,  doctorId: 1,  deptId: 1,
       date: new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc),
       notes: "Follow-up for hypertension management."),

// ── Neurology sessions (DeptId = 2, DoctorId = 2) ───────────────────
Create(  patientId: 13,  doctorId: 2,  deptId: 2,
       date: new DateTime(2024, 2, 5, 10, 30, 0, DateTimeKind.Utc),
       notes: "Migraine with aura. MRI of the brain recommended."),

Create( patientId: 14,  doctorId: 2,  deptId: 2,
       date: new DateTime(2024, 2, 12, 14, 0, 0, DateTimeKind.Utc),
       notes: "Peripheral neuropathy work-up. Blood glucose and B12 ordered."),

// ── Orthopedics sessions (DeptId = 3, DoctorId = 3) ─────────────────
Create(  patientId: 15,  doctorId: 3,  deptId: 3,
       date: new DateTime(2024, 2, 20, 8, 0, 0, DateTimeKind.Utc),
       notes: "Knee pain. X-ray shows mild osteoarthritis."),

Create(  patientId: 16,  doctorId: 3,  deptId: 3,
       date: new DateTime(2024, 3, 1, 9, 30, 0, DateTimeKind.Utc),
       notes: "Post-operative check after hip replacement."),

// ── Pediatrics sessions (DeptId = 4, DoctorId = 4) ──────────────────
Create(  patientId: 17,  doctorId: 4,  deptId: 4,
       date: new DateTime(2024, 3, 8, 10, 0, 0, DateTimeKind.Utc),
       notes: "Routine growth assessment. Vaccination updated."),

// ── Internal Medicine sessions (DeptId = 8, DoctorId = 8) ────────────
Create(  patientId: 18,  doctorId: 8,  deptId: 8,
       date: new DateTime(2024, 3, 14, 13, 0, 0, DateTimeKind.Utc),
       notes: "Type 2 Diabetes follow-up. HbA1c and lipid profile ordered."),

Create(  patientId: 19, doctorId: 8,  deptId: 8,
       date: new DateTime(2024, 3, 21, 15, 0, 0, DateTimeKind.Utc),
       notes: "Fatigue and weight loss. CBC and thyroid panel ordered."),

Create( patientId: 20, doctorId: 8,  deptId: 8,
       date: new DateTime(2024, 4, 2, 9, 0, 0, DateTimeKind.Utc),
       notes: "Hypertension assessment. Electrolytes and KFT ordered."),

// ── Dermatology sessions (DeptId = 9, DoctorId = 9) ─────────────────
Create( patientId: 21,  doctorId: 9,  deptId: 9,
       date: new DateTime(2024, 4, 10, 11, 30, 0, DateTimeKind.Utc),
       notes: "Chronic urticaria. LFT and CBC for baseline."),

// ── General Surgery sessions (DeptId = 7, DoctorId = 7) ─────────────
Create( patientId: 22, doctorId: 7,  deptId: 7,
       date: new DateTime(2024, 4, 18, 8, 30, 0, DateTimeKind.Utc),
       notes: "Pre-operative assessment for laparoscopic cholecystectomy."),
       };

        await context.Sessions.AddRangeAsync(sessions);
        await context.SaveChangesAsync();
    }

    private static Session Create(int patientId, int doctorId, int deptId,
                                  DateTime date, string? notes = null) =>
        new Session(patientId, doctorId, deptId, date, notes)
        {

            CreatedAt = DateTime.UtcNow
        };
}
