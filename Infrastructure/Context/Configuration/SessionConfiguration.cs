using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context.Configuration
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");

            builder.HasKey(x => x.SessionId);

            builder.Property(x => x.SessionDate)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(2000);

            builder.HasOne(x => x.Patient)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Doctor)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Department)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.DeptId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
