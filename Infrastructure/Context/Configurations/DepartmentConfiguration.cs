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

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.DepartmentMangager)
                .IsRequired()
                .HasMaxLength(150);

            // Head doctor: optional 1:1, no inverse navigation on Doctor.
            // Restrict delete: removing a doctor should not silently delete
            // the department; the head must be reassigned/cleared first.
            builder.HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enforces at most one department per head doctor.
            // SQL Server unique indexes allow any number of NULLs, so
            // departments with no head yet are not affected.
            builder.HasIndex(d => d.DoctorId)
                .IsUnique();

            // Staff doctors belonging to this department (1:many)
            builder.HasMany(d => d.Doctors)
                .WithOne(doc => doc.Department)
                .HasForeignKey(doc => doc.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
