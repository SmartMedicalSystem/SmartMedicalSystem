using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            // Properties
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.HeadDoctor)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.FloorNumber)
                .IsRequired(false);

           

            //builder.ToTable("Departments", t =>
            //    t.HasCheckConstraint("CK_Departments_FloorNumber_Range", "[FloorNumber] IS NULL OR ([FloorNumber] >= 0 AND [FloorNumber] <= 50)"));

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            // Head Doctor relationship
            builder.HasOne(d => d.HeadDoctorEntity)
                .WithMany()
                .HasForeignKey(d => d.HeadDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.HeadDoctorId)
                .IsUnique()
                .HasDatabaseName("IX_Departments_HeadDoctorId_Unique");

            // Staff doctors
            builder.HasMany(d => d.Doctors)
                .WithOne(doc => doc.Department)
                .HasForeignKey(doc => doc.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter
            builder.HasQueryFilter(d => !d.IsDeleted);

            // Performance indexes
            builder.HasIndex(d => d.Name)
                .HasDatabaseName("IX_Departments_Name");

            builder.HasIndex(d => d.Status)
                .HasDatabaseName("IX_Departments_Status");
        }
    }
}