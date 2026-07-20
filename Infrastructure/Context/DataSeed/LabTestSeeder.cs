using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class LabTestSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.LabTests.AnyAsync())
            return;

        var laboratoryIds = await context.Laboratories
            .Where(l => l.Code != null)
            .ToDictionaryAsync(l => l.Code!, l => l.Id);

        if (laboratoryIds.Count == 0)
            return;

        int LaboratoryId(string code) => laboratoryIds[code];

        var labTests = new List<LabTest>
        {
            Create("Complete Blood Count (CBC)",
                "Measures red cells, white cells, hemoglobin, hematocrit and platelets to screen for anemia, infection and blood disorders.",
                LaboratoryId("LAB-HEMA")),

            Create("Liver Function Tests (LFT)",
                "Panel of enzymes and proteins that assess liver health including ALT, AST, ALP, bilirubin and total protein.",
                LaboratoryId("LAB-CHEM")),

            Create("Kidney Function Tests (KFT)",
                "Evaluates kidney health through creatinine, BUN and uric acid measurements.",
                LaboratoryId("LAB-CHEM")),

            Create("Lipid Profile",
                "Measures total cholesterol, HDL, LDL and triglycerides to assess cardiovascular disease risk.",
                LaboratoryId("LAB-CHEM")),

            Create("Blood Glucose Profile",
                "Screens for diabetes and monitors glycemic control via fasting glucose, post-prandial glucose and HbA1c.",
                LaboratoryId("LAB-CHEM")),

            Create("Thyroid Function Tests (TFT)",
                "Evaluates thyroid gland activity by measuring TSH, Free T3 and Free T4.",
                LaboratoryId("LAB-CHEM")),

            Create("Electrolytes Panel",
                "Measures serum sodium, potassium, chloride and calcium to assess fluid and electrolyte balance.",
                LaboratoryId("LAB-CHEM"))
        };

        await context.LabTests.AddRangeAsync(labTests);
        await context.SaveChangesAsync();

        if (await context.LabTestElements.AnyAsync())
            return;

        var testsByName = labTests.ToDictionary(t => t.TestName, t => t.Id);
        var elementsByName = await context.TestElements
            .ToDictionaryAsync(e => e.ElementName, e => e.Id);

        var joins = new List<LabTestElement>();

        void AddElements(string testName, params string[] elementNames)
        {
            var labTestId = testsByName[testName];
            joins.AddRange(elementNames.Select(elementName =>
                new LabTestElement(labTestId, elementsByName[elementName])));
        }

        AddElements("Complete Blood Count (CBC)",
            "Hemoglobin",
            "White Blood Cells (WBC)",
            "Red Blood Cells (RBC)",
            "Platelets",
            "Hematocrit (HCT)",
            "Mean Corpuscular Volume");

        AddElements("Liver Function Tests (LFT)",
            "ALT (SGPT)",
            "AST (SGOT)",
            "Alkaline Phosphatase",
            "Total Bilirubin",
            "Direct Bilirubin",
            "Total Protein",
            "Albumin");

        AddElements("Kidney Function Tests (KFT)",
            "Serum Creatinine",
            "Blood Urea Nitrogen (BUN)",
            "Uric Acid");

        AddElements("Lipid Profile",
            "Total Cholesterol",
            "HDL Cholesterol",
            "LDL Cholesterol",
            "Triglycerides");

        AddElements("Blood Glucose Profile",
            "Fasting Blood Glucose",
            "HbA1c",
            "Post-Prandial Glucose");

        AddElements("Thyroid Function Tests (TFT)",
            "TSH",
            "Free T3",
            "Free T4");

        AddElements("Electrolytes Panel",
            "Sodium (Na)",
            "Potassium (K)",
            "Chloride (Cl)",
            "Calcium (Ca)");

        await context.LabTestElements.AddRangeAsync(joins);
        await context.SaveChangesAsync();
    }

    private static LabTest Create(string name, string description, int laboratoryId)
    {
        var labTest = new LabTest(name, description)
        {
            CreatedAt = DateTime.UtcNow
        };

        labTest.AssignToLaboratory(laboratoryId);
        return labTest;
    }
}
