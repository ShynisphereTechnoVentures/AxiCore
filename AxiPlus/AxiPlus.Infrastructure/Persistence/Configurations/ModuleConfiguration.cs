using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AxiPlus.Domain.Entities;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{        
    public void Configure(EntityTypeBuilder<Module> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasMany(x => x.Lessons)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Resources)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
