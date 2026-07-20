using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LabTechnicianConfiguration
        : IEntityTypeConfiguration<LabTechnician>
    {
        public void Configure(
            EntityTypeBuilder<LabTechnician> builder)
        {
            builder.ToTable("LabTechnicians");

            builder.HasKey(t => t.Id);

            // =========================
            // Properties
            // =========================

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Contact)
                .IsRequired()
                .HasMaxLength(50);

            // =========================
            // Laboratory Relationship
            // Laboratory 1 ---> N Technicians
            // =========================

            builder.HasOne(t => t.Laboratory)
                .WithMany(l => l.Technicians)
                .HasForeignKey(t => t.LaboratoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Soft Delete
            // =========================

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}