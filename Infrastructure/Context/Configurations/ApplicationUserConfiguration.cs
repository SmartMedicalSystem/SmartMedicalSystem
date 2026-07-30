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




            // Configure the relationship between ApplicationUser and BasePerson 
            builder.HasOne(u => u.Person).WithOne(p => p.User)
                .HasForeignKey<ApplicationUser>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete


            // User -> UserRoles relationship (Identity also configures this, but keep explicit mapping)
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}
