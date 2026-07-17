using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Data.Configurations
{
    public class LabTechnicianConfiguration : IEntityTypeConfiguration<LabTechnician>
    {
        public void Configure(EntityTypeBuilder<LabTechnician> builder)
        {
            builder.ToTable("LabTechnicians");

            // Key is configured on the root BasePerson type (see BasePersonConfiguration)

            // Personal Information

            builder.Property(t => t.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Nationality)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.EncryptedNationalId)
                .HasMaxLength(200)
                .IsRequired();

            // Employment

        
            builder.Property(t => t.Laboratory)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.JobTitle)
                .HasMaxLength(100)
                .IsRequired();

            // Contact

            builder.Property(t => t.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.AlternativePhone)
                .HasMaxLength(20);

            builder.Property(t => t.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(t => t.Address)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(t => t.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Country)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.PostalCode)
                .HasMaxLength(20);

            // Account

            builder.Property(t => t.Username)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.PhotoUrl)
                .HasMaxLength(500);

            // Soft Delete

            // NOTE: The HasQueryFilter for IsDeleted is applied on the root
            // BasePerson entity. Removing duplicate filters on derived types
            // prevents EF Core InvalidOperationException when mapping the
            // inheritance hierarchy.
        }
    }
}