using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds PatientResult and PatientResultElement rows for completed lab requests.
/// Requires Patients, Sessions, LabTests, TestElements and LabTechnicians to exist.
/// Only seeded for RequestLabs with status = Completed (sessions 1–4).
/// </summary>
public static class PatientResultSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.PatientResults.AnyAsync())
            return;

        // ── PatientResult rows ────────────────────────────────────────────────────
        //   PatientResult: (patientId, sessionId, labTestId, summary, aiReport, aiSuggestion)
        var results = new List<PatientResult>
        {
            // Session 1 – Patient 1, CBC
            CreateResult(  patientId: 1,  sessionId: 1,  labTestId: 1,
                summary: "CBC within normal limits. No signs of anemia or infection.",
                aiReport: "Normal CBC profile.",
                aiSuggestion: "No action required. Routine follow-up in 6 months."),

            // Session 1 – Patient 1, Lipid Profile
            CreateResult(  patientId: 1,  sessionId: 1,  labTestId: 4,
                summary: "Borderline high total cholesterol (210 mg/dL). LDL slightly elevated.",
                aiReport: "Mild dyslipidemia detected.",
                aiSuggestion: "Dietary modification recommended. Consider statin therapy if no improvement in 3 months."),

            // Session 2 – Patient 4, Lipid Profile
            CreateResult(  patientId: 4,  sessionId: 2,  labTestId: 4,
                summary: "Lipid profile within target range for hypertensive patient.",
                aiReport: "Lipid profile acceptable.",
                aiSuggestion: "Continue current antihypertensive regimen and low-sodium diet."),

            // Session 2 – Patient 4, Electrolytes
            CreateResult(  patientId: 4,  sessionId: 2,  labTestId: 7,
                summary: "Electrolytes balanced. Potassium slightly low at 3.3 mEq/L.",
                aiReport: "Mild hypokalemia noted.",
                aiSuggestion: "Increase dietary potassium. Recheck in 2 weeks."),

            // Session 3 – Patient 2, CBC
            CreateResult(  patientId: 2,  sessionId: 3,  labTestId: 1,
                summary: "CBC normal. Hemoglobin 13.5 g/dL, no leukocytosis.",
                aiReport: "Normal CBC.",
                aiSuggestion: "CBC does not explain migraine symptoms. Proceed with neurological imaging."),

            // Session 4 – Patient 7, Blood Glucose
            CreateResult(  patientId: 7,  sessionId: 4,  labTestId: 5,
                summary: "Fasting glucose 112 mg/dL – pre-diabetic range. HbA1c 5.9%.",
                aiReport: "Pre-diabetes indicators present.",
                aiSuggestion: "Lifestyle modification. Low-glycaemic diet and exercise program. Re-test in 3 months."),

            // Session 4 – Patient 7, CBC
            CreateResult(  patientId: 7,  sessionId: 4,  labTestId: 1,
                summary: "CBC shows mild macrocytic anemia (MCV 102 fL, Hgb 11.1 g/dL).",
                aiReport: "Macrocytic anemia – likely B12 or folate deficiency.",
                aiSuggestion: "Check serum B12 and folate levels. Start empiric supplementation if deficiency confirmed."),
        };

        await context.PatientResults.AddRangeAsync(results);
        await context.SaveChangesAsync();

        // ── PatientResultElement rows ─────────────────────────────────────────────
        if (await context.PatientResultElements.AnyAsync())
            return;

        var elements = new List<PatientResultElement>
        {
            // ── Result 1: Patient 1 – CBC (techId = 1) ───────────────────────────
            CreateElement( patientResultId: 1, testElementId: 1,  value: 14.5,  techId: 1), // Hemoglobin
            CreateElement( patientResultId: 1, testElementId: 2,  value: 7.2,   techId: 1), // WBC
            CreateElement( patientResultId: 1, testElementId: 3,  value: 5.1,   techId: 1), // RBC
            CreateElement( patientResultId: 1, testElementId: 4,  value: 230.0, techId: 1), // Platelets
            CreateElement( patientResultId: 1, testElementId: 5,  value: 43.0,  techId: 1), // Hematocrit
            CreateElement( patientResultId: 1, testElementId: 6,  value: 88.0,  techId: 1), // MCV

            // ── Result 2: Patient 1 – Lipid Profile (techId = 2) ────────────────
            CreateElement(patientResultId: 2, testElementId: 17, value: 210.0, techId: 2), // Total Cholesterol
            CreateElement(patientResultId: 2, testElementId: 18, value: 45.0,  techId: 2), // HDL
            CreateElement(patientResultId: 2, testElementId: 19, value: 128.0, techId: 2), // LDL
            CreateElement(patientResultId: 2, testElementId: 20, value: 145.0, techId: 2), // Triglycerides

            // ── Result 3: Patient 4 – Lipid Profile (techId = 1) ────────────────
            CreateElement( patientResultId: 3, testElementId: 17, value: 185.0, techId: 1), // Total Cholesterol
            CreateElement( patientResultId: 3, testElementId: 18, value: 52.0,  techId: 1), // HDL
            CreateElement( patientResultId: 3, testElementId: 19, value: 96.0,  techId: 1), // LDL
            CreateElement( patientResultId: 3, testElementId: 20, value: 130.0, techId: 1), // Triglycerides

            // ── Result 4: Patient 4 – Electrolytes (techId = 3) ─────────────────
            CreateElement( patientResultId: 4, testElementId: 27, value: 140.0, techId: 3), // Sodium
            CreateElement( patientResultId: 4, testElementId: 28, value: 3.3,   techId: 3), // Potassium (low)
            CreateElement( patientResultId: 4, testElementId: 29, value: 100.0, techId: 3), // Chloride
            CreateElement( patientResultId: 4, testElementId: 30, value: 9.2,   techId: 3), // Calcium

            // ── Result 5: Patient 2 – CBC (techId = 2) ──────────────────────────
            CreateElement( patientResultId: 5, testElementId: 1,  value: 13.5,  techId: 2), // Hemoglobin
            CreateElement( patientResultId: 5, testElementId: 2,  value: 6.8,   techId: 2), // WBC
            CreateElement( patientResultId: 5, testElementId: 3,  value: 4.7,   techId: 2), // RBC
            CreateElement( patientResultId: 5, testElementId: 4,  value: 210.0, techId: 2), // Platelets
            CreateElement( patientResultId: 5, testElementId: 5,  value: 40.0,  techId: 2), // Hematocrit
            CreateElement( patientResultId: 5, testElementId: 6,  value: 85.0,  techId: 2), // MCV

            // ── Result 6: Patient 7 – Blood Glucose (techId = 4) ────────────────
            CreateElement( patientResultId: 6, testElementId: 21, value: 112.0, techId: 4), // Fasting Glucose
            CreateElement( patientResultId: 6, testElementId: 22, value: 5.9,   techId: 4), // HbA1c
            CreateElement( patientResultId: 6, testElementId: 23, value: 148.0, techId: 4), // Post-Prandial

            // ── Result 7: Patient 7 – CBC (techId = 4) ──────────────────────────
            CreateElement( patientResultId: 7, testElementId: 1,  value: 11.1,  techId: 4), // Hemoglobin (low)
            CreateElement( patientResultId: 7, testElementId: 2,  value: 5.9,   techId: 4), // WBC
            CreateElement( patientResultId: 7, testElementId: 3,  value: 4.2,   techId: 4), // RBC
            CreateElement( patientResultId: 7, testElementId: 4,  value: 190.0, techId: 4), // Platelets
            CreateElement( patientResultId: 7, testElementId: 5,  value: 34.0,  techId: 4), // Hematocrit (low)
            CreateElement( patientResultId: 7, testElementId: 6,  value: 102.0, techId: 4), // MCV (high – macrocytic)
        };

        await context.PatientResultElements.AddRangeAsync(elements);
        await context.SaveChangesAsync();
    }

    private static PatientResult CreateResult( int patientId, int sessionId, int labTestId,
                                              string summary, string aiReport, string aiSuggestion) =>
        new PatientResult(patientId, sessionId, labTestId, summary, aiReport, aiSuggestion)
        {
           
            CreatedAt = DateTime.UtcNow
        };

    private static PatientResultElement CreateElement( int patientResultId,
                                                      int testElementId, double value, int techId) =>
        new PatientResultElement(patientResultId, testElementId, value, techId)
        {
        
            CreatedAt = DateTime.UtcNow
        };
}
