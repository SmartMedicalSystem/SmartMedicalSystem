using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LaboratoryConfiguration
        : IEntityTypeConfiguration<Laboratory>
    {
        public void Configure(
            EntityTypeBuilder<Laboratory> builder)
        {
            builder.ToTable("Laboratories");

            builder.HasKey(l => l.Id);

            // =========================
            // Properties
            // =========================

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Location)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Phone)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(LabStatus.Active);

            builder.Property(l => l.Code)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(l => l.Specialty)
                .IsRequired(false)
                .HasMaxLength(100);

            // =========================
            // Department Relationship
            // Department 1 ---> N Laboratories
            // =========================

            builder.HasOne(l => l.Department)
                .WithMany(d => d.Laboratories)
                .HasForeignKey(l => l.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Head Technician Relationship
            // Laboratory 0..1 ---> Head Technician
            // =========================

            builder.HasOne(l => l.HeadTechnician)
                .WithMany()
                .HasForeignKey(l => l.HeadTechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Technicians Relationship
            // Laboratory 1 ---> N Technicians
            // =========================

            builder.HasMany(l => l.Technicians)
                .WithOne(t => t.Laboratory)
                .HasForeignKey(t => t.LaboratoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Lab Tests Relationship
            // Laboratory 1 ---> N LabTests
            // =========================

            builder.HasMany(l => l.LabTests)
                .WithOne(lt => lt.Laboratory)
                .HasForeignKey(lt => lt.LabId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Indexes
            // =========================

            builder.HasIndex(l => l.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL")
                .HasDatabaseName(
                    "IX_Laboratories_Code_Unique");

            builder.HasIndex(l => l.Name)
                .HasDatabaseName("IX_Laboratories_Name");

            builder.HasIndex(l => l.Status)
                .HasDatabaseName("IX_Laboratories_Status");

            // =========================
            // Soft Delete
            // =========================

            builder.HasQueryFilter(l => !l.IsDeleted);
        }
    }
}