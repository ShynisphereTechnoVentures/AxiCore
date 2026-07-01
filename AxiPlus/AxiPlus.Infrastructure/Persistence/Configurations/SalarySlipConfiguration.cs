using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxiPlus.Infrastructure.Persistence.Configurations;

public class SalarySlipConfiguration : IEntityTypeConfiguration<SalarySlip>
{        
    public void Configure(EntityTypeBuilder<SalarySlip> builder)
   {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.FileUrl).HasMaxLength(1000);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UploadedByUser)
            .WithMany()
            .HasForeignKey(x => x.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
