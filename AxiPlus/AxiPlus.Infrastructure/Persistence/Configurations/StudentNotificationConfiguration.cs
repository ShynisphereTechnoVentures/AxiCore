using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class StudentNotificationConfiguration
    : IEntityTypeConfiguration<StudentNotification>
{       
    public void Configure(EntityTypeBuilder<StudentNotification> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(40)
            .IsRequired();

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
