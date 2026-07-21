using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PatientResultElementConfiguration : IEntityTypeConfiguration<PatientResultElement>
    {
        public void Configure(EntityTypeBuilder<PatientResultElement> builder)
        {
            builder.ToTable("PatientResultElements");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Value)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Comment)
                .HasMaxLength(500);

            // The relationship to PatientResult (via PatientResultId) is
            // configured once, from PatientResultConfiguration, to avoid
            // configuring the same relationship from both sides.

            builder.HasOne(e => e.Element)
                .WithMany(el => el.PatientResultElements)
                .HasForeignKey(e => e.ElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Technician)
                .WithMany()
                .HasForeignKey(e => e.TechId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
