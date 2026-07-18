using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds LabTest catalogue entries AND the LabTestElement join rows that link each test
/// to its constituent TestElements. TestElements must already exist before this runs.
/// </summary>
public static class LabTestSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.LabTests.AnyAsync())
            return;

        // ── 1. Seed LabTest catalogue ────────────────────────────────────────────
        var labTests = new List<LabTest>
       {
           Create(  "Complete Blood Count (CBC)",
                    "Measures red cells, white cells, hemoglobin, hematocrit and platelets to screen for anemia, infection and blood disorders."),

           Create(  "Liver Function Tests (LFT)",
                    "Panel of enzymes and proteins that assess liver health including ALT, AST, ALP, bilirubin and total protein."),

           Create(  "Kidney Function Tests (KFT)",
                    "Evaluates kidney health through creatinine, BUN and uric acid measurements."),

           Create(  "Lipid Profile",
                    "Measures total cholesterol, HDL, LDL and triglycerides to assess cardiovascular disease risk."),

           Create(  "Blood Glucose Profile",
                    "Screens for diabetes and monitors glycemic control via fasting glucose, post-prandial glucose and HbA1c."),

           Create(  "Thyroid Function Tests (TFT)",
                    "Evaluates thyroid gland activity by measuring TSH, Free T3 and Free T4."),

           Create(  "Electrolytes Panel",
                      "Measures serum sodium, potassium, chloride and calcium to assess fluid and electrolyte balance."),
       };

        await context.LabTests.AddRangeAsync(labTests);
        await context.SaveChangesAsync();

        // ── 2. Seed LabTestElement (join) rows ───────────────────────────────────
        if (await context.LabTestElements.AnyAsync())
            return;

        var joins = new List<LabTestElement>
       {
           // CBC → elements 1–6
           new LabTestElement(1, 1),   // Hemoglobin
           new LabTestElement(1, 2),   // WBC
           new LabTestElement(1, 3),   // RBC
           new LabTestElement(1, 4),   // Platelets
           new LabTestElement(1, 5),   // Hematocrit
           new LabTestElement(1, 6),   // MCV

           // LFT → elements 7–13
           new LabTestElement(2, 7),   // ALT
           new LabTestElement(2, 8),   // AST
           new LabTestElement(2, 9),   // ALP
           new LabTestElement(2, 10),  // Total Bilirubin
           new LabTestElement(2, 11),  // Direct Bilirubin
           new LabTestElement(2, 12),  // Total Protein
           new LabTestElement(2, 13),  // Albumin

           // KFT → elements 14–16
           new LabTestElement(3, 14),  // Creatinine
           new LabTestElement(3, 15),  // BUN
           new LabTestElement(3, 16),  // Uric Acid

           // Lipid Profile → elements 17–20
           new LabTestElement(4, 17),  // Total Cholesterol
           new LabTestElement(4, 18),  // HDL
           new LabTestElement(4, 19),  // LDL
           new LabTestElement(4, 20),  // Triglycerides

           // Blood Glucose → elements 21–23
           new LabTestElement(5, 21),  // Fasting Glucose
           new LabTestElement(5, 22),  // HbA1c
           new LabTestElement(5, 23),  // Post-Prandial Glucose

           // Thyroid → elements 24–26
           new LabTestElement(6, 24),  // TSH
           new LabTestElement(6, 25),  // Free T3
           new LabTestElement(6, 26),  // Free T4

           // Electrolytes → elements 27–30
           new LabTestElement(7, 27),  // Sodium
           new LabTestElement(7, 28),  // Potassium
           new LabTestElement(7, 29),  // Chloride
           new LabTestElement(7, 30),  // Calcium
       };

        await context.LabTestElements.AddRangeAsync(joins);
        await context.SaveChangesAsync();
    }

    private static LabTest Create(string name, string description) =>
        new LabTest(name, description)
        {

            CreatedAt = DateTime.UtcNow
        };
}
