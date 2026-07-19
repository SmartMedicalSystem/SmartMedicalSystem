using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LabTestConfiguration : IEntityTypeConfiguration<LabTest>
    {
        public void Configure(EntityTypeBuilder<LabTest> builder)
        {
            builder.ToTable("LabTests");

            builder.HasKey(lt => lt.Id);

            builder.Property(lt => lt.TestName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(lt => lt.Description)
                .HasMaxLength(500);

            builder.HasMany(lt => lt.LabTestElements)
                .WithOne(lte => lte.LabTest)
                .HasForeignKey(lte => lte.LabTestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
