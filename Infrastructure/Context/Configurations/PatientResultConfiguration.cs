using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PatientResultConfiguration : IEntityTypeConfiguration<PatientResult>
    {
        public void Configure(EntityTypeBuilder<PatientResult> builder)
        {
            builder.ToTable("PatientResults");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.AIClassifiedReport)
                .HasMaxLength(4000);

            builder.Property(r => r.AISuggestion)
                .HasMaxLength(4000);

            builder.Property(r => r.Summary)
                .HasMaxLength(2000);

            builder.HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Session)
                .WithMany()
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property is named "labTest" (lowercase) on the entity - kept as-is from the source.
            builder.HasOne(r => r.labTest)
                .WithMany()
                .HasForeignKey(r => r.LabTestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Result elements only ever exist as part of a result: cascade here is intentional composition,
            // unlike the Restrict used above for references to independent master/reference data.
            builder.HasMany(r => r.ResultElements)
                .WithOne(e => e.patientResult)
                .HasForeignKey(e => e.PatientResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
