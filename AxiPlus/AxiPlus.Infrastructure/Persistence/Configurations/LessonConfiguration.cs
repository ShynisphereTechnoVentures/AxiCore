using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AxiPlus.Domain.Entities;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{     
    public void Configure(EntityTypeBuilder<Lesson> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Content)
            .HasMaxLength(10000);
    }
}