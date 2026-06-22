using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class RequestLabsConfiguration : IEntityTypeConfiguration<RequestLabs>
    {
        public void Configure(EntityTypeBuilder<RequestLabs> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
             .IsRequired();

            builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

            builder.HasOne(x => x.Session)
              .WithMany(x => x.RequestLabs)
              .HasForeignKey(x => x.SessionId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LabTest)
               .WithMany(x => x.RequestLabs)
               .HasForeignKey(x => x.LabTestId)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
