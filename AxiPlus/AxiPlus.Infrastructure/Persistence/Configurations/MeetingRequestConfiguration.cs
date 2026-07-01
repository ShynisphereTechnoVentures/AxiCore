using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class MeetingRequestConfiguration : IEntityTypeConfiguration<MeetingRequest>
{    
    public void Configure(EntityTypeBuilder<MeetingRequest> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MeetingLink).HasMaxLength(1000);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.StudentResponseNote).HasMaxLength(1000);
        builder.Property(x => x.MentorFollowUpNote).HasMaxLength(1000);

        builder.HasOne(x => x.MentorUser)
            .WithMany()
            .HasForeignKey(x => x.MentorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Batch)
            .WithMany()
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
