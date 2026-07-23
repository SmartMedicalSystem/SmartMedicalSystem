using Domain.Entities.Person;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Context.Configurations
{
    public class BasePersonConfiguration
        : IEntityTypeConfiguration<BasePerson>
    {
        public void Configure(EntityTypeBuilder<BasePerson> builder)
        {
            builder.ToTable("BasePersons");

            builder.HasKey(x => x.Id);

            builder.Ignore(p => p.Name);

            builder.HasQueryFilter(p => !p.IsDeleted);






        }
    }
}