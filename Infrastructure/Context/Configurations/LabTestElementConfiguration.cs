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
                lte.TestElementId
            });

            builder.HasOne(lte => lte.LabTest)
                .WithMany(lt => lt.LabTestElements)
                .HasForeignKey(lte => lte.LabTestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lte => lte.TestElement)
                .WithMany(te => te.LabTestElements)
                .HasForeignKey(lte => lte.TestElementId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
