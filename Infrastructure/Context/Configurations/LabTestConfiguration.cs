using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LabTestConfiguration
        : IEntityTypeConfiguration<LabTest>
    {
        public void Configure(
            EntityTypeBuilder<LabTest> builder)
        {
            builder.ToTable("LabTests");

            builder.HasKey(lt => lt.Id);

            // =========================
            // Properties
            // =========================

            builder.Property(lt => lt.TestName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(lt => lt.Description)
                .IsRequired()
                .HasMaxLength(500);

            // =========================
            // Laboratory Relationship
            // Laboratory 1 ---> N LabTests
            // =========================

            builder.HasOne(lt => lt.Laboratory)
                .WithMany(l => l.LabTests)
                .HasForeignKey(lt => lt.LaboratoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // LabTestElements Relationship
            // LabTest 1 ---> N LabTestElements
            // =========================

            builder.HasMany(lt => lt.LabTestElements)
                .WithOne(lte => lte.LabTest)
                .HasForeignKey(lte => lte.LabTestId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Soft Delete
            // =========================

            builder.HasQueryFilter(lt => !lt.IsDeleted);
        }
    }
}