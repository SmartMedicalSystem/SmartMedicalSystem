using Domain.Entities.Baseperson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BasePersonConfiguration : IEntityTypeConfiguration<BasePerson>
    {
        public void Configure(EntityTypeBuilder<BasePerson> builder)
        {
            // Configure the key on the root of the inheritance hierarchy.
            builder.HasKey(p => p.Id);

            // Common person properties
            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(50);

            builder.Property(p => p.Email)
                .HasMaxLength(150);

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
