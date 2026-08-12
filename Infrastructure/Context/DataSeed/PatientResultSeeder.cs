using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class PatientResultSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.PatientResults.AnyAsync())
            return;

        var sessions = await context.Sessions
            .OrderBy(s => s.SessionDate)
            .ToListAsync();
        var labTests = await context.LabTests
            .ToDictionaryAsync(t => t.TestName, t => t.Id);

        if (sessions.Count < 4 || labTests.Count < 7)
            return;

        Session Session(int position) => sessions[position - 1];
        int LabTestId(string name) => labTests[name];

        var results = new List<PatientResult>
        {
            CreateResult(Session(1).PatientId, Session(1).Id, LabTestId("Complete Blood Count (CBC)"),
                "CBC within normal limits. No signs of anemia or infection.",
                "Normal CBC profile.",
                "No action required. Routine follow-up in 6 months."),

            CreateResult(Session(1).PatientId, Session(1).Id, LabTestId("Lipid Profile"),
                "Borderline high total cholesterol (210 mg/dL). LDL slightly elevated.",
                "Mild dyslipidemia detected.",
                "Dietary modification recommended. Consider statin therapy if no improvement in 3 months."),

            CreateResult(Session(2).PatientId, Session(2).Id, LabTestId("Lipid Profile"),
                "Lipid profile within target range for hypertensive patient.",
                "Lipid profile acceptable.",
                "Continue current antihypertensive regimen and low-sodium diet."),

            CreateResult(Session(2).PatientId, Session(2).Id, LabTestId("Electrolytes Panel"),
                "Electrolytes balanced. Potassium slightly low at 3.3 mEq/L.",
                "Mild hypokalemia noted.",
                "Increase dietary potassium. Recheck in 2 weeks."),

            CreateResult(Session(3).PatientId, Session(3).Id, LabTestId("Complete Blood Count (CBC)"),
                "CBC normal. Hemoglobin 13.5 g/dL, no leukocytosis.",
                "Normal CBC.",
                "CBC does not explain migraine symptoms. Proceed with neurological imaging."),

            CreateResult(Session(4).PatientId, Session(4).Id, LabTestId("Blood Glucose Profile"),
                "Fasting glucose 112 mg/dL - pre-diabetic range. HbA1c 5.9%.",
                "Pre-diabetes indicators present.",
                "Lifestyle modification. Low-glycaemic diet and exercise program. Re-test in 3 months."),

            CreateResult(Session(4).PatientId, Session(4).Id, LabTestId("Complete Blood Count (CBC)"),
                "CBC shows mild macrocytic anemia (MCV 102 fL, Hgb 11.1 g/dL).",
                "Macrocytic anemia - likely B12 or folate deficiency.",
                "Check serum B12 and folate levels. Start empiric supplementation if deficiency confirmed.")
        };

        await context.PatientResults.AddRangeAsync(results);
        await context.SaveChangesAsync();

        if (await context.PatientResultElements.AnyAsync())
            return;

        var elements = await context.TestElements
            .ToDictionaryAsync(e => e.ElementName, e => e.Id);
        var technicians = await context.LabTechnicians
            .Where(t => t.Email != null)
            .ToDictionaryAsync(t => t.Email!, t => t.Id);

        int ElementId(string name) => elements[name];
        int TechId(string email) => technicians[email];

        var resultElements = new List<PatientResultElement>
        {
            CreateElement(results[0].Id, ElementId("Hemoglobin"), 14.5, TechId("mohamed48289@gmail.com")),
            CreateElement(results[0].Id, ElementId("White Blood Cells (WBC)"), 7.2, TechId("mohamed48289@gmail.com")),
            CreateElement(results[0].Id, ElementId("Red Blood Cells (RBC)"), 5.1, TechId("mohamed48289@gmail.com")),
            CreateElement(results[0].Id, ElementId("Platelets"), 230.0, TechId("mohamed48289@gmail.com")),
            CreateElement(results[0].Id, ElementId("Hematocrit (HCT)"), 43.0, TechId("mohamed48289@gmail.com")),
            CreateElement(results[0].Id, ElementId("Mean Corpuscular Volume"), 88.0, TechId("mohamed48289@gmail.com")),

            CreateElement(results[1].Id, ElementId("Total Cholesterol"), 210.0, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[1].Id, ElementId("HDL Cholesterol"), 45.0, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[1].Id, ElementId("LDL Cholesterol"), 128.0, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[1].Id, ElementId("Triglycerides"), 145.0, TechId("lotfykhattab95@gmail.com")),

            CreateElement(results[2].Id, ElementId("Total Cholesterol"), 185.0, TechId("mohamed48289@gmail.com")),
            CreateElement(results[2].Id, ElementId("HDL Cholesterol"), 52.0, TechId("mohamed48289@gmail.com")),
            CreateElement(results[2].Id, ElementId("LDL Cholesterol"), 96.0, TechId("mohamed48289@gmail.com")),
            CreateElement(results[2].Id, ElementId("Triglycerides"), 130.0, TechId("mohamed48289@gmail.com")),

            CreateElement(results[3].Id, ElementId("Sodium (Na)"), 140.0, TechId("akramuhammad95@gmail.com")),
            CreateElement(results[3].Id, ElementId("Potassium (K)"), 3.3, TechId("akramuhammad95@gmail.com")),
            CreateElement(results[3].Id, ElementId("Chloride (Cl)"), 100.0, TechId("akramuhammad95@gmail.com")),
            CreateElement(results[3].Id, ElementId("Calcium (Ca)"), 9.2, TechId("akramuhammad95@gmail.com")),

            CreateElement(results[4].Id, ElementId("Hemoglobin"), 13.5, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[4].Id, ElementId("White Blood Cells (WBC)"), 6.8, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[4].Id, ElementId("Red Blood Cells (RBC)"), 4.7, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[4].Id, ElementId("Platelets"), 210.0, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[4].Id, ElementId("Hematocrit (HCT)"), 40.0, TechId("lotfykhattab95@gmail.com")),
            CreateElement(results[4].Id, ElementId("Mean Corpuscular Volume"), 85.0, TechId("lotfykhattab95@gmail.com")),

            CreateElement(results[5].Id, ElementId("Fasting Blood Glucose"), 112.0, TechId("hana.wael@medsystem.local")),
            CreateElement(results[5].Id, ElementId("HbA1c"), 5.9, TechId("hana.wael@medsystem.local")),
            CreateElement(results[5].Id, ElementId("Post-Prandial Glucose"), 148.0, TechId("hana.wael@medsystem.local")),

            CreateElement(results[6].Id, ElementId("Hemoglobin"), 11.1, TechId("ramy.elsayed@medsystem.local")),
            CreateElement(results[6].Id, ElementId("White Blood Cells (WBC)"), 5.9, TechId("ramy.elsayed@medsystem.local")),
            CreateElement(results[6].Id, ElementId("Red Blood Cells (RBC)"), 4.2, TechId("ramy.elsayed@medsystem.local")),
            CreateElement(results[6].Id, ElementId("Platelets"), 190.0, TechId("ramy.elsayed@medsystem.local")),
            CreateElement(results[6].Id, ElementId("Hematocrit (HCT)"), 34.0, TechId("ramy.elsayed@medsystem.local")),
            CreateElement(results[6].Id, ElementId("Mean Corpuscular Volume"), 102.0, TechId("ramy.elsayed@medsystem.local"))
        };

        await context.PatientResultElements.AddRangeAsync(resultElements);
        await context.SaveChangesAsync();
    }

    private static PatientResult CreateResult(int patientId, int sessionId, int labTestId,
        string summary, string aiReport, string aiSuggestion) =>
        new PatientResult(patientId, sessionId, labTestId, summary, aiReport, aiSuggestion)
        {
            CreatedAt = DateTime.UtcNow
        };

    private static PatientResultElement CreateElement(int patientResultId,
        int testElementId, double value, int techId) =>
        new PatientResultElement(patientResultId, testElementId, value, techId)
        {
            CreatedAt = DateTime.UtcNow
        };
}
