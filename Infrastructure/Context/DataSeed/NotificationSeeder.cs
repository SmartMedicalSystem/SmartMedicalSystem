using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

public static class NotificationSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.Notifications.AnyAsync())
            return;

        // Resolve users that are known to exist after the other seeders run
        var adminUser = await userManager.FindByEmailAsync("mohamedsaiedhassan308@gmail.com");
        var drAhmed = await userManager.FindByEmailAsync("mohamed48289@gmail.com");
        var drSara = await userManager.FindByEmailAsync("sara.mohamed@medsystem.local");
        var drHeba = await userManager.FindByEmailAsync("salwasayed522@gmail.com");

        var drAhmed =
            await userManager.FindByEmailAsync(
                "ahmed.hassan@medsystem.local");

        var drSara =
            await userManager.FindByEmailAsync(
                "sara.mohamed@medsystem.local");

        var drHeba =
            await userManager.FindByEmailAsync(
                "heba.salah@medsystem.local");

        var labTechAmir =
            await userManager.FindByNameAsync(
                "tech.amir.hassan");

        if (adminUser is null || drAhmed is null)
            return;

        var patientResults = await context.PatientResults
            .OrderBy(x => x.Id)
            .Take(7)
            .ToListAsync();

        var requestLabs = await context.RequestLabs
            .OrderBy(x => x.Id)
            .Take(2)
            .ToListAsync();

        var firstPatientResult =
            patientResults.FirstOrDefault();

        var thirdPatientResult =
            patientResults.Count >= 3
                ? patientResults[2]
                : firstPatientResult;

        var firstRequestLab =
            requestLabs.FirstOrDefault();

        var secondRequestLab =
            requestLabs.Count >= 2
                ? requestLabs[1]
                : firstRequestLab;

        var notifications = new List<Notification>();

        void Add(
            int userId,
            string message,
            DateTime sentAt,
            NotificationType type,
            bool isRead = false,
            int? requestLabsId = null,
            int? patientResultId = null)
        {
            var notification = new Notification(
                userId,
                message,
                type,
                requestLabsId,
                patientResultId);

            notification.SentAt = sentAt;
            notification.CreatedAt = sentAt;

            if (isRead)
                notification.MarkAsRead();

            notifications.Add(notification);
        }

        Add(
            adminUser.Id,
            "System initialized successfully. All roles and permissions seeded.",
            new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc),
            NotificationType.AIReportGenerated,
            true);

        Add(
            adminUser.Id,
            "New doctor accounts registered: Dr. Ahmed Hassan, Dr. Sara Mohamed.",
            new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc),
            NotificationType.AppointmentReminder,
            true);

        Add(
            adminUser.Id,
            "Monthly activity report ready. 12 sessions conducted in January.",
            new DateTime(2024, 2, 1, 8, 0, 0, DateTimeKind.Utc),
            NotificationType.AIReportGenerated,
            false);

        Add(
            adminUser.Id,
            "Reminder: Lab equipment maintenance scheduled for next week.",
            new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc),
            NotificationType.AppointmentReminder,
            false);

        if (drAhmed is not null)
        {
            Add(
                drAhmed.Id,
                "Lab results are ready for review.",
                new DateTime(2024, 1, 11, 10, 0, 0, DateTimeKind.Utc),
                NotificationType.LabResultReady,
                true,
                patientResultId: firstPatientResult?.Id);

            Add(
                drAhmed.Id,
                "Appointment reminder: Follow-up with Patient Nour Ahmed tomorrow at 11:00.",
                new DateTime(2024, 1, 14, 17, 0, 0, DateTimeKind.Utc),
                NotificationType.AppointmentReminder,
                true);

            Add(
                drAhmed.Id,
                "AI Report generated successfully.",
                new DateTime(2024, 1, 11, 12, 0, 0, DateTimeKind.Utc),
                NotificationType.AIReportGenerated,
                false,
                patientResultId: firstPatientResult?.Id);
        }

        if (drSara is not null)
        {
            Add(
                drSara.Id,
                "Laboratory results are now available.",
                new DateTime(2024, 2, 6, 9, 0, 0, DateTimeKind.Utc),
                NotificationType.LabResultReady,
                true,
                patientResultId: thirdPatientResult?.Id);

            Add(
                drSara.Id,
                "Reminder: Patient Hossam Maher follow-up scheduled for Feb 19.",
                new DateTime(2024, 2, 12, 16, 0, 0, DateTimeKind.Utc),
                NotificationType.AppointmentReminder,
                false);
        }

        if (drHeba is not null)
        {
            Add(
                drHeba.Id,
                "A new laboratory request is in progress.",
                new DateTime(2024, 3, 14, 14, 0, 0, DateTimeKind.Utc),
                NotificationType.LabTestRequested,
                true,
                requestLabsId: firstRequestLab?.Id);

            Add(
                drHeba.Id,
                "Pending laboratory results are ready for review.",
                new DateTime(2024, 3, 22, 8, 30, 0, DateTimeKind.Utc),
                NotificationType.LabResultReady,
                false,
                patientResultId: thirdPatientResult?.Id);

            Add(
                drHeba.Id,
                "New session has been booked.",
                new DateTime(2024, 3, 30, 11, 0, 0, DateTimeKind.Utc),
                NotificationType.AppointmentReminder,
                false);
        }

        if (labTechAmir is not null)
        {
            Add(
                labTechAmir.Id,
                "New laboratory request received.",
                new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                NotificationType.LabTestRequested,
                false,
                requestLabsId: firstRequestLab?.Id);

            Add(
                labTechAmir.Id,
                "Laboratory results have been submitted successfully.",
                new DateTime(2024, 1, 11, 10, 30, 0, DateTimeKind.Utc),
                NotificationType.LabResultReady,
                true,
                patientResultId: firstPatientResult?.Id);

            Add(
                labTechAmir.Id,
                "AI report generated successfully.",
                new DateTime(2024, 1, 11, 11, 0, 0, DateTimeKind.Utc),
                NotificationType.AIReportGenerated,
                false,
                patientResultId: firstPatientResult?.Id);

            Add(
                labTechAmir.Id,
                "A new laboratory request is waiting to be processed.",
                new DateTime(2024, 3, 14, 14, 0, 0, DateTimeKind.Utc),
                NotificationType.LabTestRequested,
                false,
                requestLabsId: secondRequestLab?.Id);

            Add(
                labTechAmir.Id,
                "Patient laboratory result has been reviewed successfully.",
                new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc),
                NotificationType.LabResultReady,
                true,
                patientResultId: firstPatientResult?.Id);
        }

        if (notifications.Count == 0)
            return;

        await context.Notifications.AddRangeAsync(notifications);

        await context.SaveChangesAsync();
    }
}