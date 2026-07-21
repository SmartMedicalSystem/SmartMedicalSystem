using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context.Configurations
{
    public class LabTestElementConfiguration : IEntityTypeConfiguration<LabTestElement>
    {
        public void Configure(EntityTypeBuilder<LabTestElement> builder)
        {
            builder.ToTable("LabTestElements");

            builder.HasKey(lte => new
            {
                lte.LabTestId,
                lte.ElementId
            });

            builder.Property(lte => lte.DisplayOrder)
                .IsRequired();

            builder.Property(lte => lte.IsRequired)
                .IsRequired();

            builder.HasOne(lte => lte.LabTest)
                .WithMany(lt => lt.LabTestElements)
                .HasForeignKey(lte => lte.LabTestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lte => lte.Element)
                .WithMany(e => e.LabTestElements)
                .HasForeignKey(lte => lte.ElementId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
