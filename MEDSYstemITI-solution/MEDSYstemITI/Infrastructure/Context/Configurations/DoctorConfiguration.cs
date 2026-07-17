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

            // Key is configured on the root BasePerson type (see BasePersonConfiguration)

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Gender)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            // The Department (staff membership) relationship is configured
            // once, from DepartmentConfiguration (HasMany/WithOne), to avoid
            // configuring the same relationship from both sides.

            // NOTE: Do not set a query filter on derived types. A global
            // query filter for IsDeleted is configured on the root of the
            // inheritance hierarchy (BasePersonConfiguration). Applying
            // the same HasQueryFilter here causes EF Core InvalidOperationException.
        }
    }
}
