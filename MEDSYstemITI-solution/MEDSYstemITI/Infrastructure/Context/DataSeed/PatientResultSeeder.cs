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

        var sessions = await context.Sessions.OrderBy(s => s.Id).ToListAsync();
        var labTests = await context.LabTests.OrderBy(t => t.Id).ToListAsync();

        if (sessions.Count < 4 || labTests.Count < 7)
            return; // Prerequisites not seeded yet.

        Session Session(int position) => sessions[position - 1];
        LabTest Test(int position) => labTests[position - 1];

        var results = new List<PatientResult>
        {
            CreateResult(patientId: Session(1).PatientId, sessionId: Session(1).Id, labTestId: Test(1).Id,
                summary: "CBC within normal limits. No signs of anemia or infection.",
                aiReport: "Normal CBC profile.",
                aiSuggestion: "No action required. Routine follow-up in 6 months."),

            CreateResult(patientId: Session(1).PatientId, sessionId: Session(1).Id, labTestId: Test(4).Id,
                summary: "Borderline high total cholesterol (210 mg/dL). LDL slightly elevated.",
                aiReport: "Mild dyslipidemia detected.",
                aiSuggestion: "Dietary modification recommended. Consider statin therapy if no improvement in 3 months."),

            CreateResult(patientId: Session(2).PatientId, sessionId: Session(2).Id, labTestId: Test(4).Id,
                summary: "Lipid profile within target range for hypertensive patient.",
                aiReport: "Lipid profile acceptable.",
                aiSuggestion: "Continue current antihypertensive regimen and low-sodium diet."),

            CreateResult(patientId: Session(2).PatientId, sessionId: Session(2).Id, labTestId: Test(7).Id,
                summary: "Electrolytes balanced. Potassium slightly low at 3.3 mEq/L.",
                aiReport: "Mild hypokalemia noted.",
                aiSuggestion: "Increase dietary potassium. Recheck in 2 weeks."),

            CreateResult(patientId: Session(3).PatientId, sessionId: Session(3).Id, labTestId: Test(1).Id,
                summary: "CBC normal. Hemoglobin 13.5 g/dL, no leukocytosis.",
                aiReport: "Normal CBC.",
                aiSuggestion: "CBC does not explain migraine symptoms. Proceed with neurological imaging."),

            CreateResult(patientId: Session(4).PatientId, sessionId: Session(4).Id, labTestId: Test(5).Id,
                summary: "Fasting glucose 112 mg/dL – pre-diabetic range. HbA1c 5.9%.",
                aiReport: "Pre-diabetes indicators present.",
                aiSuggestion: "Lifestyle modification. Low-glycaemic diet and exercise program. Re-test in 3 months."),

            CreateResult(patientId: Session(4).PatientId, sessionId: Session(4).Id, labTestId: Test(1).Id,
                summary: "CBC shows mild macrocytic anemia (MCV 102 fL, Hgb 11.1 g/dL).",
                aiReport: "Macrocytic anemia – likely B12 or folate deficiency.",
                aiSuggestion: "Check serum B12 and folate levels. Start empiric supplementation if deficiency confirmed."),
        };

        await context.PatientResults.AddRangeAsync(results);
        await context.SaveChangesAsync();

        if (await context.PatientResultElements.AnyAsync())
            return;

        var elements = new List<PatientResultElement>
        {
            CreateElement(patientResultId: results[0].Id, testElementId: 1,  value: 14.5,  techId: 23),
            CreateElement(patientResultId: results[0].Id, testElementId: 2,  value: 7.2,   techId: 23),
            CreateElement(patientResultId: results[0].Id, testElementId: 3,  value: 5.1,   techId: 23),
            CreateElement(patientResultId: results[0].Id, testElementId: 4,  value: 230.0, techId: 23),
            CreateElement(patientResultId: results[0].Id, testElementId: 5,  value: 43.0,  techId: 23),
            CreateElement(patientResultId: results[0].Id, testElementId: 6,  value: 88.0,  techId: 23),

            CreateElement(patientResultId: results[1].Id, testElementId: 17, value: 210.0, techId: 24),
            CreateElement(patientResultId: results[1].Id, testElementId: 18, value: 45.0,  techId: 24),
            CreateElement(patientResultId: results[1].Id, testElementId: 19, value: 128.0, techId: 24),
            CreateElement(patientResultId: results[1].Id, testElementId: 20, value: 145.0, techId: 24),

            CreateElement(patientResultId: results[2].Id, testElementId: 17, value: 185.0, techId: 23),
            CreateElement(patientResultId: results[2].Id, testElementId: 18, value: 52.0,  techId: 23),
            CreateElement(patientResultId: results[2].Id, testElementId: 19, value: 96.0,  techId: 23),
            CreateElement(patientResultId: results[2].Id, testElementId: 20, value: 130.0, techId: 23),

            CreateElement(patientResultId: results[3].Id, testElementId: 27, value: 140.0, techId: 25),
            CreateElement(patientResultId: results[3].Id, testElementId: 28, value: 3.3,   techId: 25),
            CreateElement(patientResultId: results[3].Id, testElementId: 29, value: 100.0, techId: 25),
            CreateElement(patientResultId: results[3].Id, testElementId: 30, value: 9.2,   techId: 25),

            CreateElement(patientResultId: results[4].Id, testElementId: 1,  value: 13.5,  techId: 24),
            CreateElement(patientResultId: results[4].Id, testElementId: 2,  value: 6.8,   techId: 24),
            CreateElement(patientResultId: results[4].Id, testElementId: 3,  value: 4.7,   techId: 24),
            CreateElement(patientResultId: results[4].Id, testElementId: 4,  value: 210.0, techId: 24),
            CreateElement(patientResultId: results[4].Id, testElementId: 5,  value: 40.0,  techId: 24),
            CreateElement(patientResultId: results[4].Id, testElementId: 6,  value: 85.0,  techId: 24),

            CreateElement(patientResultId: results[5].Id, testElementId: 21, value: 112.0, techId: 26),
            CreateElement(patientResultId: results[5].Id, testElementId: 22, value: 5.9,   techId: 26),
            CreateElement(patientResultId: results[5].Id, testElementId: 23, value: 148.0, techId: 26),

            CreateElement(patientResultId: results[6].Id, testElementId: 1,  value: 11.1,  techId: 27),
            CreateElement(patientResultId: results[6].Id, testElementId: 2,  value: 5.9,   techId: 27),
            CreateElement(patientResultId: results[6].Id, testElementId: 3,  value: 4.2,   techId: 27),
            CreateElement(patientResultId: results[6].Id, testElementId: 4,  value: 190.0, techId: 27),
            CreateElement(patientResultId: results[6].Id, testElementId: 5,  value: 34.0,  techId: 27),
            CreateElement(patientResultId: results[6].Id, testElementId: 6,  value: 102.0, techId: 27),
        };

        await context.PatientResultElements.AddRangeAsync(elements);
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