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

            //builder.HasKey(p => p.Id);

            // "Name" is a computed pass-through over FirstName/LastName (see BasePerson.Name)
            // and must NOT be mapped to its own column.
            builder.Ignore(p => p.Name);

            // FirstName, LastName and Age now have public getters (private
            // setters) on Patient after the domain-validation pass, so they
            // can be mapped with normal typed lambdas.
            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            //builder.Property(p => p.Age);

            builder.Property(p => p.DateOfBirth)
                .IsRequired();

            //builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
