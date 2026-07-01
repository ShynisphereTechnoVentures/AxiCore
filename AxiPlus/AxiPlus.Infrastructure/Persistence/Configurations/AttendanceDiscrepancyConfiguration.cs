using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class AttendanceDiscrepancyConfiguration :
    IEntityTypeConfiguration<AttendanceDiscrepancy>
{       
    public void Configure(EntityTypeBuilder<AttendanceDiscrepancy> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.ResolutionNote).HasMaxLength(1000);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ReviewedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
