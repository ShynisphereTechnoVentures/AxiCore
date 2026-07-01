using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class MentorProfileConfiguration : IEntityTypeConfiguration<MentorProfile>
{       
    public void Configure(EntityTypeBuilder<MentorProfile> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PhoneNumber).HasMaxLength(40);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.EmergencyContact).HasMaxLength(120);
        builder.Property(x => x.Designation).HasMaxLength(120);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId).IsUnique();
    }
}
