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

            //builder.HasKey(d => d.Id);

            // "Name" is a computed pass-through over FirstName/LastName (see BasePerson.Name)
            // and must NOT be mapped to its own column, or EF will try to persist a value
            // that duplicates/conflicts with FirstName+LastName every time the model changes.
            builder.Ignore(d => d.Name);

            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Email)
                .HasMaxLength(150);

            builder.Property(d => d.Address)
                .HasMaxLength(250);

            builder.Property(d => d.Nationality)
                .HasMaxLength(100);

            builder.Property(d => d.EncryptedNationalId)
                .HasMaxLength(200);

            builder.Property(d => d.Gender)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            // The Department (staff membership) relationship is configured
            // once, from DepartmentConfiguration (HasMany/WithOne), to avoid
            // configuring the same relationship from both sides.

            //builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
