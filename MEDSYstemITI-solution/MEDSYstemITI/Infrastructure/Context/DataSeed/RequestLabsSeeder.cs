using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class RequestLabsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.RequestLabs.AnyAsync())
            return;

        // Resolve by insertion order instead of literal Ids, so this doesn't
        // break if Sessions/LabTests end up with non-contiguous or shifted
        // identity values.
        var sessions = await context.Sessions.OrderBy(s => s.Id).ToListAsync();
        var labTests = await context.LabTests.OrderBy(t => t.Id).ToListAsync();

        if (sessions.Count < 12 || labTests.Count < 7)
            return; // Prerequisites not seeded yet.

        Session Session(int position) => sessions[position - 1];
        LabTest Test(int position) => labTests[position - 1];

        var requests = new List<RequestLabs>();

        // Session 1 – Cardiology → CBC + Lipid Profile
        var req1 = CreateRequest(sessionId: Session(1).Id,
            date: new DateTime(2024, 1, 10, 9, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req1.LabTests.Add(Test(1));
        req1.LabTests.Add(Test(4));
        requests.Add(req1);

        // Session 2 – Cardiology follow-up → Lipid Profile + Electrolytes
        var req2 = CreateRequest(sessionId: Session(2).Id,
            date: new DateTime(2024, 1, 15, 11, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req2.LabTests.Add(Test(4));
        req2.LabTests.Add(Test(7));
        requests.Add(req2);

        // Session 3 – Neurology (migraine) → CBC
        var req3 = CreateRequest(sessionId: Session(3).Id,
            date: new DateTime(2024, 2, 5, 11, 0, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req3.LabTests.Add(Test(1));
        requests.Add(req3);

        // Session 4 – Neurology (neuropathy) → Blood Glucose + CBC
        var req4 = CreateRequest(sessionId: Session(4).Id,
            date: new DateTime(2024, 2, 12, 14, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Completed);
        req4.LabTests.Add(Test(5));
        req4.LabTests.Add(Test(1));
        requests.Add(req4);

        // Session 8 – Internal Medicine (Diabetes) → Blood Glucose + Lipid
        var req5 = CreateRequest(sessionId: Session(8).Id,
            date: new DateTime(2024, 3, 14, 13, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.InProgress);
        req5.LabTests.Add(Test(5));
        req5.LabTests.Add(Test(4));
        requests.Add(req5);

        // Session 9 – Internal Medicine → CBC + Thyroid + LFT
        var req6 = CreateRequest(sessionId: Session(9).Id,
            date: new DateTime(2024, 3, 21, 15, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req6.LabTests.Add(Test(1));
        req6.LabTests.Add(Test(6));
        req6.LabTests.Add(Test(2));
        requests.Add(req6);

        // Session 10 – Internal Medicine (Hypertension) → Electrolytes + KFT
        var req7 = CreateRequest(sessionId: Session(10).Id,
            date: new DateTime(2024, 4, 2, 9, 30, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req7.LabTests.Add(Test(7));
        req7.LabTests.Add(Test(3));
        requests.Add(req7);

        // Session 11 – Dermatology → LFT + CBC
        var req8 = CreateRequest(sessionId: Session(11).Id,
            date: new DateTime(2024, 4, 10, 12, 0, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req8.LabTests.Add(Test(2));
        req8.LabTests.Add(Test(1));
        requests.Add(req8);

        // Session 12 – General Surgery (pre-op) → CBC + LFT + KFT + Electrolytes
        var req9 = CreateRequest(sessionId: Session(12).Id,
            date: new DateTime(2024, 4, 18, 9, 0, 0, DateTimeKind.Utc),
            status: LabRequestStatus.Pending);
        req9.LabTests.Add(Test(1));
        req9.LabTests.Add(Test(2));
        req9.LabTests.Add(Test(3));
        req9.LabTests.Add(Test(7));
        requests.Add(req9);

        await context.RequestLabs.AddRangeAsync(requests);
        await context.SaveChangesAsync();
    }

    private static RequestLabs CreateRequest(int sessionId, DateTime date, LabRequestStatus status)
    {
        var req = new RequestLabs(sessionId, date)
        {
            CreatedAt = DateTime.UtcNow
        };
        req.UpdateStatus(status);
        return req;
    }
}