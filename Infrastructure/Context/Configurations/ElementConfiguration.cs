using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Context.Configurations
{
    public class ElementConfiguration : IEntityTypeConfiguration<Element>
    {
        public void Configure(EntityTypeBuilder<Element> builder)
        {
            builder.ToTable("Elements");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Unit)
                .HasMaxLength(50);

            builder.Property(e => e.ReferenceRange)
                .HasMaxLength(150);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.HasIndex(e => e.Name);

            builder.HasMany(e => e.LabTestElements)
                .WithOne(lte => lte.Element)
                .HasForeignKey(lte => lte.ElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.PatientResultElements)
                .WithOne(pre => pre.Element)
                .HasForeignKey(pre => pre.ElementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
