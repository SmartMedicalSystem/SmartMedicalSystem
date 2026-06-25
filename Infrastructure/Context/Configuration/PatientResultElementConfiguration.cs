using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context.Configuration
{
    public class PatientResultElementConfiguration
     : IEntityTypeConfiguration<PatientResultElement>
    {
        public void Configure(EntityTypeBuilder<PatientResultElement> builder)
        {
            builder.ToTable("PatientResultElements");

            builder.HasKey(x => x.ElementId);

            builder.Property(x => x.Value)
                   .IsRequired();

            builder.HasOne(x => x.Result)
                   .WithMany(x => x.ResultElements)
                   .HasForeignKey(x => x.ResultId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TestElement)
                   .WithMany(x => x.PatientResultElements)
                   .HasForeignKey(x => x.TestElementId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Technician)
                   .WithMany(x => x.PatientResultElements)
                   .HasForeignKey(x => x.TechId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
