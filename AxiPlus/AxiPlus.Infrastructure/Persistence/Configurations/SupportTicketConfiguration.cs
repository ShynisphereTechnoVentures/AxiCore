using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class SupportTicketConfiguration
    : IEntityTypeConfiguration<SupportTicket>
{       
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.MentorResponse)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
