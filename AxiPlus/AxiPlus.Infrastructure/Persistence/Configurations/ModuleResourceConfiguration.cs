using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AxiPlus.Domain.Entities;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class ModuleResourceConfiguration : IEntityTypeConfiguration<ModuleResource>
{        
    public void Configure(EntityTypeBuilder<ModuleResource> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Url)
            .IsRequired();
    }
}