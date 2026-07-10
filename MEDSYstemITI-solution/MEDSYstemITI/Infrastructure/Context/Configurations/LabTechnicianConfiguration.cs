using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Data.Configurations
{
    public class LabTechnicianConfiguration : IEntityTypeConfiguration<LabTechnician>
    {
        public void Configure(EntityTypeBuilder<LabTechnician> builder)
        {
            builder.ToTable("LabTechnicians");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(150);

            builder.Property(t => t.Contact)
                .HasMaxLength(50);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
