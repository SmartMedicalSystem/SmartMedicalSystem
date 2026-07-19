using Domain.Entities;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeed;

/// <summary>
/// Seeds system Notifications for existing users (Admin + Doctors).
/// Requires users to already exist in the AspNetUsers table.
/// </summary>
public static class NotificationSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.Notifications.AnyAsync())
            return;

        // Resolve users that are known to exist after the other seeders run
        var adminUser = await userManager.FindByEmailAsync("admin@medsystem.local");
        var drAhmed = await userManager.FindByEmailAsync("ahmed.hassan@medsystem.local");
        var drSara = await userManager.FindByEmailAsync("sara.mohamed@medsystem.local");
        var drHeba = await userManager.FindByEmailAsync("heba.salah@medsystem.local");

        if (adminUser is null || drAhmed is null)
            return; // Skip if users haven't been seeded yet

        var notifications = new List<Notification>();


        void Add(int userId, string message, DateTime sentAt, bool isRead = false)
        {
            var n = new Notification(userId, message, sentAt) { CreatedAt = sentAt };
            if (isRead) n.MarkAsRead();
            notifications.Add(n);
        }

        // ── Admin notifications ───────────────────────────────────────────────────
        Add(adminUser.Id, "System initialized successfully. All roles and permissions seeded.",
            new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc), isRead: true);

        Add(adminUser.Id, "New doctor accounts registered: Dr. Ahmed Hassan, Dr. Sara Mohamed.",
            new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc), isRead: true);

        Add(adminUser.Id, "Monthly activity report ready. 12 sessions conducted in January.",
            new DateTime(2024, 2, 1, 8, 0, 0, DateTimeKind.Utc), isRead: false);

        Add(adminUser.Id, "Reminder: Lab equipment maintenance scheduled for next week.",
            new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc), isRead: false);

        // ── Dr. Ahmed Hassan notifications ────────────────────────────────────────
        if (drAhmed is not null)
        {
            Add(drAhmed.Id, "Lab results ready for Patient Mohamed Youssef (Session #1) – CBC & Lipid Profile.",
                new DateTime(2024, 1, 11, 10, 0, 0, DateTimeKind.Utc), isRead: true);

            Add(drAhmed.Id, "Appointment reminder: Follow-up with Patient Nour Ahmed tomorrow at 11:00.",
                new DateTime(2024, 1, 14, 17, 0, 0, DateTimeKind.Utc), isRead: true);

            Add(drAhmed.Id, "AI Report generated: Mild dyslipidemia detected for Patient #1.",
                new DateTime(2024, 1, 11, 12, 0, 0, DateTimeKind.Utc), isRead: false);
        }

        // ── Dr. Sara Mohamed notifications ────────────────────────────────────────
        if (drSara is not null)
        {
            Add(drSara.Id, "CBC results for Patient Fatima Hassan (Session #3) are now available.",
                new DateTime(2024, 2, 6, 9, 0, 0, DateTimeKind.Utc), isRead: true);

            Add(drSara.Id, "Reminder: Patient Hossam Maher follow-up scheduled for Feb 19.",
                new DateTime(2024, 2, 12, 16, 0, 0, DateTimeKind.Utc), isRead: false);
        }

        // ── Dr. Heba Salah notifications ──────────────────────────────────────────
        if (drHeba is not null)
        {
            Add(drHeba.Id, "Lab request #5 (Patient Karim Samir) is in progress.",
                new DateTime(2024, 3, 14, 14, 0, 0, DateTimeKind.Utc), isRead: true);

            Add(drHeba.Id, "Pending lab results for Patient Dina Mustafa (Session #9) – please review.",
                new DateTime(2024, 3, 22, 8, 30, 0, DateTimeKind.Utc), isRead: false);

            Add(drHeba.Id, "New session booked: Patient Mahmoud Refaat on Apr 2 at 09:00.",
                new DateTime(2024, 3, 30, 11, 0, 0, DateTimeKind.Utc), isRead: false);
        }

        await context.Notifications.AddRangeAsync(notifications);
        await context.SaveChangesAsync();
    }
}
