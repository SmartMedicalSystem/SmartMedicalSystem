using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RequestLabTestConfiguration : IEntityTypeConfiguration<RequestLabTest>
    {
        public void Configure(EntityTypeBuilder<RequestLabTest> builder)
        {
            builder.ToTable("RequestLabTests");

            // Composite key: (RequestLabId, LabTestId)
            builder.HasKey(rlt => new { rlt.RequestLabId, rlt.LabTestId });

            builder.Property(rlt => rlt.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(rlt => rlt.CreatedAt)
                .IsRequired();

            builder.Property(rlt => rlt.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(rlt => rlt.RequestLab)
                .WithMany(r => r.RequestLabTests)
                .HasForeignKey(rlt => rlt.RequestLabId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rlt => rlt.LabTest)
                .WithMany(lt => lt.RequestLabTests)
                .HasForeignKey(rlt => rlt.LabTestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
