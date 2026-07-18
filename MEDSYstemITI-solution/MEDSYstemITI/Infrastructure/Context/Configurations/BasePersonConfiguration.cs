using Domain.Entities;
using Domain.Entities.Baseperson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context.Configurations
{
    public class BasePersonConfiguration : IEntityTypeConfiguration<BasePerson>
    {
        public void Configure(EntityTypeBuilder<BasePerson> builder)
        {
            builder.ToTable("BasePersons");
            builder.HasKey(x => x.Id);

            builder.Ignore(p => p.Name);

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
