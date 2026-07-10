using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.Contact)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Gender)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            // The Department (staff membership) relationship is configured
            // once, from DepartmentConfiguration (HasMany/WithOne), to avoid
            // configuring the same relationship from both sides.

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
