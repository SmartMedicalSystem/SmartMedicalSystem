using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds RequestLabs rows. Each row represents a doctor-ordered lab request attached
/// to an existing Session. Requires Sessions and LabTests to already exist.
/// The many-to-many between RequestLabs and LabTest is managed via EF Core's
/// automatic join table (RequestLabs.LabTests collection).
/// </summary>
public static class RequestLabsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.RequestLabs.AnyAsync())
            return;

        // Load navigations so EF Core can track the many-to-many relationship
        var labTests = await context.LabTests.ToListAsync();

        LabTest GetTest(int id) =>
            labTests.First(t => t.Id == id);

        // ── Build RequestLabs with their LabTest associations ────────────────────
        var requests = new List<RequestLabs>();

        // Session 1 – Cardiology (patient 1, Dr. Ahmed) → CBC + Lipid Profile
        var req1 = CreateRequest(sessionId: 1,
            date: new DateTime(2024, 1, 10, 9, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req1.LabTests.Add(GetTest(1)); // CBC
        req1.LabTests.Add(GetTest(4)); // Lipid Profile
        requests.Add(req1);

        // Session 2 – Cardiology follow-up (patient 4) → Lipid Profile + Electrolytes
        var req2 = CreateRequest(sessionId: 2,
            date: new DateTime(2024, 1, 15, 11, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req2.LabTests.Add(GetTest(4)); // Lipid Profile
        req2.LabTests.Add(GetTest(7)); // Electrolytes
        requests.Add(req2);

        // Session 3 – Neurology (patient 2, migraine) → CBC
        var req3 = CreateRequest(sessionId: 3,
            date: new DateTime(2024, 2, 5, 11, 0, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req3.LabTests.Add(GetTest(1)); // CBC
        requests.Add(req3);

        // Session 4 – Neurology (patient 7, neuropathy) → Blood Glucose + CBC
        var req4 = CreateRequest(sessionId: 4,
            date: new DateTime(2024, 2, 12, 14, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req4.LabTests.Add(GetTest(5)); // Blood Glucose
        req4.LabTests.Add(GetTest(1)); // CBC
        requests.Add(req4);

        // Session 8 – Internal Medicine (patient 5, Diabetes) → Blood Glucose + Lipid
        var req5 = CreateRequest(sessionId: 8,
            date: new DateTime(2024, 3, 14, 13, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.InProgress);
        req5.LabTests.Add(GetTest(5)); // Blood Glucose
        req5.LabTests.Add(GetTest(4)); // Lipid Profile
        requests.Add(req5);

        // Session 9 – Internal Medicine (patient 10) → CBC + Thyroid + LFT
        var req6 = CreateRequest( sessionId: 9,
            date: new DateTime(2024, 3, 21, 15, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req6.LabTests.Add(GetTest(1)); // CBC
        req6.LabTests.Add(GetTest(6)); // Thyroid
        req6.LabTests.Add(GetTest(2)); // LFT
        requests.Add(req6);

        // Session 10 – Internal Medicine (patient 11, Hypertension) → Electrolytes + KFT
        var req7 = CreateRequest( sessionId: 10,
            date: new DateTime(2024, 4, 2, 9, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req7.LabTests.Add(GetTest(7)); // Electrolytes
        req7.LabTests.Add(GetTest(3)); // KFT
        requests.Add(req7);

        // Session 11 – Dermatology (patient 6) → LFT + CBC
        var req8 = CreateRequest( sessionId: 11,
            date: new DateTime(2024, 4, 10, 12, 0, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req8.LabTests.Add(GetTest(2)); // LFT
        req8.LabTests.Add(GetTest(1)); // CBC
        requests.Add(req8);

        // Session 12 – General Surgery (patient 12, pre-op) → CBC + LFT + KFT + Electrolytes
        var req9 = CreateRequest( sessionId: 12,
            date: new DateTime(2024, 4, 18, 9, 0, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req9.LabTests.Add(GetTest(1)); // CBC
        req9.LabTests.Add(GetTest(2)); // LFT
        req9.LabTests.Add(GetTest(3)); // KFT
        req9.LabTests.Add(GetTest(7)); // Electrolytes
        requests.Add(req9);

        await context.RequestLabs.AddRangeAsync(requests);
        await context.SaveChangesAsync();
    }

    private static RequestLabs CreateRequest( int sessionId,
                                             DateTime date, LabRequestStatus status)
    {
        var req = new RequestLabs(sessionId, date)
        {
      
            CreatedAt = DateTime.UtcNow
        };
        req.UpdateStatus(status);
        return req;
    }
}
