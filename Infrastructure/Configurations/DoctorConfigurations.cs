using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations
{
    public class DoctorConfigurations : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id)
           .ValueGeneratedOnAdd();

            builder.Property(d => d.Name)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Contact)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(d => d.DepartmentId)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            builder.Property(d => d.IsDeleted).HasDefaultValueSql("0");

        }
    }
}
