using Domain.Entities.Baseperson;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Context.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Keep the default Identity table name used in migrations
            builder.ToTable("AspNetUsers");

            builder.Property(u => u.FullName)
                .HasMaxLength(200);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500);

            // Relationship to the domain person (optional)
            builder.HasOne(u => u.Person)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<BasePerson>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // User -> UserRoles relationship (Identity also configures this, but keep explicit mapping)
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}
