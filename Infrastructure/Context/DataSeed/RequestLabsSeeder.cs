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

        var sessions = await context.Sessions
            .OrderBy(s => s.SessionDate)
            .ToListAsync();
        var labTests = await context.LabTests
            .ToDictionaryAsync(t => t.TestName, t => t);

        if (sessions.Count < 12 || labTests.Count < 7)
            return;

        Session Session(int position) => sessions[position - 1];
        LabTest Test(string name) => labTests[name];

        var requests = new List<RequestLabs>();

        AddRequest(Session(1), LabRequestStatus.Completed,
            new DateTime(2024, 1, 10, 9, 30, 0, DateTimeKind.Utc),
            Test("Complete Blood Count (CBC)"),
            Test("Lipid Profile"));

        AddRequest(Session(2), LabRequestStatus.Completed,
            new DateTime(2024, 1, 15, 11, 30, 0, DateTimeKind.Utc),
            Test("Lipid Profile"),
            Test("Electrolytes Panel"));

        AddRequest(Session(3), LabRequestStatus.Completed,
            new DateTime(2024, 2, 5, 11, 0, 0, DateTimeKind.Utc),
            Test("Complete Blood Count (CBC)"));

        AddRequest(Session(4), LabRequestStatus.Completed,
            new DateTime(2024, 2, 12, 14, 30, 0, DateTimeKind.Utc),
            Test("Blood Glucose Profile"),
            Test("Complete Blood Count (CBC)"));

        AddRequest(Session(8), LabRequestStatus.InProgress,
            new DateTime(2024, 3, 14, 13, 30, 0, DateTimeKind.Utc),
            Test("Blood Glucose Profile"),
            Test("Lipid Profile"));

        AddRequest(Session(9), LabRequestStatus.Pending,
            new DateTime(2024, 3, 21, 15, 30, 0, DateTimeKind.Utc),
            Test("Complete Blood Count (CBC)"),
            Test("Thyroid Function Tests (TFT)"),
            Test("Liver Function Tests (LFT)"));

        AddRequest(Session(10), LabRequestStatus.Pending,
            new DateTime(2024, 4, 2, 9, 30, 0, DateTimeKind.Utc),
            Test("Electrolytes Panel"),
            Test("Kidney Function Tests (KFT)"));

        AddRequest(Session(11), LabRequestStatus.Pending,
            new DateTime(2024, 4, 10, 12, 0, 0, DateTimeKind.Utc),
            Test("Liver Function Tests (LFT)"),
            Test("Complete Blood Count (CBC)"));

        AddRequest(Session(12), LabRequestStatus.Pending,
            new DateTime(2024, 4, 18, 9, 0, 0, DateTimeKind.Utc),
            Test("Complete Blood Count (CBC)"),
            Test("Liver Function Tests (LFT)"),
            Test("Kidney Function Tests (KFT)"),
            Test("Electrolytes Panel"));

        await context.RequestLabs.AddRangeAsync(requests);
        await context.SaveChangesAsync();

        void AddRequest(Session session, LabRequestStatus status, DateTime requestedAt, params LabTest[] tests)
        {
            var request = CreateRequest(session.Id, requestedAt, status);

            for (int i = 0; i < tests.Length; i++)
            {
                var test = tests[i];

                // Determine sensible RequestLabTest status based on overall request status
                RequestLabTestStatus rltStatus;
                DateTime createdAt = requestedAt.AddMinutes(-5 * (i + 1));
                DateTime? updatedAt = null;

                if (status == LabRequestStatus.Completed)
                {
                    rltStatus = RequestLabTestStatus.Completed;
                    updatedAt = requestedAt.AddMinutes(10);
                }
                else if (status == LabRequestStatus.InProgress)
                {
                    // first test in the request is in-progress, others pending
                    rltStatus = i == 0 ? RequestLabTestStatus.InProgress : RequestLabTestStatus.Pending;
                    if (rltStatus == RequestLabTestStatus.InProgress)
                        updatedAt = requestedAt.AddMinutes(2);
                }
                else if (status == LabRequestStatus.Pending)
                {
                    rltStatus = RequestLabTestStatus.Pending;
                }
                else
                {
                    rltStatus = RequestLabTestStatus.Cancelled;
                    updatedAt = requestedAt.AddMinutes(1);
                }

                request.RequestLabTests.Add(new RequestLabTest
                {
                    LabTest = test,
                    LabTestId = test.Id,
                    Status = rltStatus,
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt
                });
            }

            requests.Add(request);
        }
    }

    private static RequestLabs CreateRequest(int sessionId, DateTime date, LabRequestStatus status)
    {
        var request = new RequestLabs(sessionId, date)
        {
            CreatedAt = DateTime.UtcNow
        };

        request.UpdateStatus(status);
        return request;
    }
}
