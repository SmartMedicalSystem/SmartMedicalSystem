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
                .IsRequired();

            // The relationship to PatientResult (via PatientResultId) is
            // configured once, from PatientResultConfiguration, to avoid
            // configuring the same relationship from both sides.

            builder.HasOne(e => e.TestElement)
                .WithMany()
                .HasForeignKey(e => e.TestElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Technician)
                .WithMany()
                .HasForeignKey(e => e.TechId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
