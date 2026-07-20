using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration
        : IEntityTypeConfiguration<Department>
    {
        public void Configure(
            EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            // =========================
            // Properties
            // =========================

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.HeadDoctor)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.FloorNumber)
                .IsRequired(false);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            // =========================
            // Head Doctor Relationship
            // Department 0..1 ---> Doctor
            // =========================

            builder.HasOne(d => d.HeadDoctorEntity)
                .WithMany()
                .HasForeignKey(d => d.HeadDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.HeadDoctorId)
                .IsUnique()
                .HasFilter("[HeadDoctorId] IS NOT NULL")
                .HasDatabaseName(
                    "IX_Departments_HeadDoctorId_Unique");

            // =========================
            // Doctors Relationship
            // Department 1 ---> N Doctors
            // =========================

            builder.HasMany(d => d.Doctors)
                .WithOne(doc => doc.Department)
                .HasForeignKey(doc => doc.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Laboratories Relationship
            // Department 1 ---> N Laboratories
            // =========================

            builder.HasMany(d => d.Laboratories)
                .WithOne(l => l.Department)
                .HasForeignKey(l => l.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Soft Delete
            // =========================

            builder.HasQueryFilter(d => !d.IsDeleted);

            // =========================
            // Indexes
            // =========================

            builder.HasIndex(d => d.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_Departments_Name");

            builder.HasIndex(d => d.Status)
                .HasDatabaseName("IX_Departments_Status");
        }
    }
}