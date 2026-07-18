using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RequestLabsConfiguration : IEntityTypeConfiguration<RequestLabs>
    {
        public void Configure(EntityTypeBuilder<RequestLabs> builder)
        {
            builder.ToTable("RequestLabs");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.RequestedAt)
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // A request is created within, and belongs exclusively to, a single session.
            builder.HasOne(r => r.Session)
                .WithMany()
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Per clarification: one RequestLabs groups all the tests requested in a session -> many-to-many with LabTest.
            builder.HasMany(r => r.LabTests)
                .WithMany()
                .UsingEntity(j => j.ToTable("RequestLabsLabTests"));

            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
