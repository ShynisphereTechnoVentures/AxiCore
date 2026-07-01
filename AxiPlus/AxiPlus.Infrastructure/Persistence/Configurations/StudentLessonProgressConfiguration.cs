using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AxiPlus.Domain.Entities;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class StudentLessonProgressConfiguration
    : IEntityTypeConfiguration<StudentLessonProgress>
{     
    public void Configure(
        EntityTypeBuilder<StudentLessonProgress> builder)
   {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Student)
            .WithMany(x => x.LessonProgresses)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.Progresses)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ReviewedBy)
.WithMany()
.HasForeignKey(x => x.ReviewedById)
.OnDelete(DeleteBehavior.NoAction);
    }
}