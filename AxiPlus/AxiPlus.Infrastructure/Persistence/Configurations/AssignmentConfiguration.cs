using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{      
    public void Configure(EntityTypeBuilder<Assignment> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.SourceRole)
            .HasMaxLength(80);

        builder.HasOne(x => x.Batch)
            .WithMany()
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Lesson)
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedMentorUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedMentorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
