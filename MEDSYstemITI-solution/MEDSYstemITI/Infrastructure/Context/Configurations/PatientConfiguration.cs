using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            // Key is configured on the root BasePerson type (see BasePersonConfiguration)

            // FirstName, LastName and Age now have public getters (private
            // setters) on Patient after the domain-validation pass, so they
            // can be mapped with normal typed lambdas.
            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Age);

            builder.Property(p => p.DateOfBirth)
                .IsRequired();

            // NOTE: Global query filter for IsDeleted is defined on the
            // BasePerson root entity. Do not add HasQueryFilter on derived
            // types (Patient) as that causes EF Core to throw.
        }
    }
}
