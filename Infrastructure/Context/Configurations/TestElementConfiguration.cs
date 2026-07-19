using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TestElementConfiguration : IEntityTypeConfiguration<TestElement>
    {
        public void Configure(EntityTypeBuilder<TestElement> builder)
        {
            builder.ToTable("TestElements");

            builder.HasKey(te => te.Id);

            builder.Property(te => te.ElementName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(te => te.Unit)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(te => te.NormalMin)
                .IsRequired();

            builder.Property(te => te.NormalMax)
                .IsRequired();

            builder.HasMany(te => te.LabTestElements)
                .WithOne(lte => lte.TestElement)
                .HasForeignKey(lte => lte.TestElementId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
