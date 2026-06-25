using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context.Configuration
{
    public class PatientResultConfiguration : IEntityTypeConfiguration<PatientResult>
    {
        public void Configure(EntityTypeBuilder<PatientResult> builder)
        {
            builder.ToTable("PatientResults");

            builder.HasKey(x => x.ResultId);

            builder.Property(x => x.AIClassifiedReport)
                   .HasMaxLength(255);

            builder.Property(x => x.AISuggestion)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Summary)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ResultDate)
                   .IsRequired();

            builder.HasOne(x => x.Patient)
                   .WithMany(x => x.PatientResults)
                   .HasForeignKey(x => x.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Session)
                   .WithMany()
                   .HasForeignKey(x => x.SessionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Test)
                   .WithMany(x => x.PatientResults)
                   .HasForeignKey(x => x.TestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ResultElements)
                   .WithOne(x => x.Result)
                   .HasForeignKey(x => x.ResultId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
