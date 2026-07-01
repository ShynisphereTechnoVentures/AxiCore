using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class AssignmentSubmissionConfiguration
    : IEntityTypeConfiguration<AssignmentSubmission>
{       
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubmissionLink)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.Feedback)
            .HasMaxLength(2000);

        builder.HasIndex(x => new{ x.AssignmentId, x.StudentId })
            .IsUnique();

        builder.HasOne(x => x.Assignment)
            .WithMany(x => x.Submissions)
            .HasForeignKey(x => x.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
