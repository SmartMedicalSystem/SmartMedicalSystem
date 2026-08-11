using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class NotificationConfiguration
        : IEntityTypeConfiguration<Notification>
    {
        public void Configure(
            EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            // ============================================================
            // Primary Key
            // ============================================================

            builder.HasKey(n => n.Id);


            // ============================================================
            // UserId
            // ============================================================

            builder.Property(n => n.UserId)
                .IsRequired();


            // ============================================================
            // Message
            // ============================================================

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);


            // ============================================================
            // SentAt
            // ============================================================

            builder.Property(n => n.SentAt)
                .IsRequired();


            // ============================================================
            // IsRead
            // ============================================================

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);


            // ============================================================
            // Notification Type
            // ============================================================
            // Enum is stored as int in SQL Server:
            //
            // LabTestRequested     = 1
            // LabResultReady       = 2
            // AppointmentReminder  = 3
            // AIReportGenerated    = 4

            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<int>();


            // ============================================================
            // User Relationship
            // ============================================================

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================================
            // RequestLabs Relationship
            // ============================================================

            builder.HasOne(n => n.RequestLabs)
                .WithMany()
                .HasForeignKey(n => n.RequestLabsId)
                .OnDelete(DeleteBehavior.NoAction);


            // ============================================================
            // PatientResult Relationship
            // ============================================================

            builder.HasOne(n => n.PatientResult)
                .WithMany()
                .HasForeignKey(n => n.PatientResultId)
                .OnDelete(DeleteBehavior.NoAction);


            // ============================================================
            // Global Query Filter
            // ============================================================

            builder.HasQueryFilter(n => !n.IsDeleted);
        }
    }
}