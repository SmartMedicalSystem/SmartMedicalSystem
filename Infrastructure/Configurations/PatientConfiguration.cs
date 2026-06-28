using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.SSN)
                   .IsRequired()
                   .HasMaxLength(14);

            builder.HasIndex(p => p.SSN)
                   .IsUnique();

            builder.Property(p => p.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.Contact)
                   .IsRequired()
                   .HasMaxLength(11);

            builder.Property(p => p.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.DateOfBirth)
                   .IsRequired();

            builder.HasMany(p => p.Sessions)
                    .WithOne(s => s.Patient)
                    .HasForeignKey(s => s.PatientId);

            builder.HasMany(p => p.PatientResults)
                    .WithOne(pr => pr.Patient)
                    .HasForeignKey(pr => pr.PatientId);
        }
    }
}
