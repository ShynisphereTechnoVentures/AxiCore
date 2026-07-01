using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AxiPlus.Domain.Entities;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class LessonLiveClassConfiguration
    : IEntityTypeConfiguration<LessonLiveClass>
{        
    public void Configure(
        EntityTypeBuilder<LessonLiveClass> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MeetingLink)
            .IsRequired();

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.LiveClasses)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}