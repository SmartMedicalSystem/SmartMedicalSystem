using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds TestElement rows – these are the individual measurable components of a lab test
/// (e.g. Hemoglobin, Blood Glucose, Creatinine). Each element defines its normal range
/// and measurement unit. LabTestElements (the join) is seeded in LabTestSeeder.
/// </summary>
public static class TestElementSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.TestElements.AnyAsync())
            return;

        var elements = new List<TestElement>
       {
           // ── Complete Blood Count (CBC) ──────────────────────────────────────
           Create(  "Hemoglobin",              "g/dL",   12.0f,  17.5f),
           Create(  "White Blood Cells (WBC)", "10³/µL",  4.5f,  11.0f),
           Create(  "Red Blood Cells (RBC)",   "10⁶/µL",  4.5f,   5.9f),
           Create(  "Platelets",               "10³/µL", 150.0f, 400.0f),
           Create(  "Hematocrit (HCT)",        "%",       38.0f,  54.0f),
           Create(  "Mean Corpuscular Volume",  "fL",      80.0f,  96.0f),

           // ── Liver Function Tests (LFT) ──────────────────────────────────────
           Create( "ALT (SGPT)",              "U/L",     7.0f,  56.0f),
           Create( "AST (SGOT)",              "U/L",    10.0f,  40.0f),
           Create( "Alkaline Phosphatase",    "U/L",    44.0f, 147.0f),
           Create( "Total Bilirubin",          "mg/dL",   0.1f,   1.2f),
           Create( "Direct Bilirubin",         "mg/dL",   0.0f,   0.3f),
           Create( "Total Protein",            "g/dL",    6.0f,   8.3f),
           Create( "Albumin",                  "g/dL",    3.5f,   5.0f),

           // ── Kidney Function Tests (KFT) ─────────────────────────────────────
           Create( "Serum Creatinine",         "mg/dL",   0.6f,   1.2f),
           Create( "Blood Urea Nitrogen (BUN)","mg/dL",   7.0f,  20.0f),
           Create( "Uric Acid",                "mg/dL",   3.4f,   7.0f),

           // ── Lipid Profile ────────────────────────────────────────────────────
           Create("Total Cholesterol",        "mg/dL",   0.0f, 200.0f),
           Create("HDL Cholesterol",          "mg/dL",  40.0f,  60.0f),
           Create("LDL Cholesterol",          "mg/dL",   0.0f, 100.0f),
           Create("Triglycerides",            "mg/dL",   0.0f, 150.0f),

           // ── Blood Glucose ─────────────────────────────────────────────────────
           Create("Fasting Blood Glucose",    "mg/dL",  70.0f,  99.0f),
           Create("HbA1c",                    "%",       4.0f,   5.6f),
           Create("Post-Prandial Glucose",    "mg/dL",   0.0f, 140.0f),

           // ── Thyroid Function ──────────────────────────────────────────────────
           Create( "TSH",                      "mIU/L",   0.4f,   4.0f),
           Create( "Free T3",                  "pg/mL",   2.3f,   4.2f),
           Create( "Free T4",                  "ng/dL",   0.8f,   1.8f),

           // ── Electrolytes ──────────────────────────────────────────────────────
           Create("Sodium (Na)",              "mEq/L",  136.0f, 145.0f),
           Create("Potassium (K)",            "mEq/L",    3.5f,   5.0f),
           Create("Chloride (Cl)",            "mEq/L",   98.0f, 106.0f),
           Create("Calcium (Ca)",             "mg/dL",    8.5f,  10.5f),
       };

        await context.TestElements.AddRangeAsync(elements);
        await context.SaveChangesAsync();
    }

    private static TestElement Create(string name, string unit, float min, float max) =>
        new TestElement(name, unit, min, max)
        {

            CreatedAt = DateTime.UtcNow
        };
}
