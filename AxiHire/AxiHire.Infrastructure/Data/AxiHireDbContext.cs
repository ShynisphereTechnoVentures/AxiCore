using AxiHire.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AxiHire.Infrastructure.Data;

public sealed class AxiHireDbContext : DbContext
{
    public AxiHireDbContext(DbContextOptions<AxiHireDbContext> options)
        : base(options)
    {
    }

    public DbSet<CandidatePassportSnapshot> CandidatePassportSnapshots =>
        Set<CandidatePassportSnapshot>();

    public DbSet<RecruiterOrganization> RecruiterOrganizations =>
        Set<RecruiterOrganization>();

    public DbSet<CandidateVerificationInvite> CandidateVerificationInvites =>
        Set<CandidateVerificationInvite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CandidatePassportSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AxiCoreUserId).IsUnique();
            entity.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Headline).HasMaxLength(240).IsRequired();
            entity.Property(x => x.PrimarySkill).HasMaxLength(120).IsRequired();
            entity.Property(x => x.SkillSummary).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.VerificationStatus).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<RecruiterOrganization>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(180).IsRequired();
            entity.Property(x => x.ContactEmail).HasMaxLength(240).IsRequired();
        });

        modelBuilder.Entity<CandidateVerificationInvite>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(80).IsRequired();
            entity.HasOne(x => x.CandidatePassportSnapshot)
                .WithMany()
                .HasForeignKey(x => x.CandidatePassportSnapshotId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.RecruiterOrganization)
                .WithMany()
                .HasForeignKey(x => x.RecruiterOrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
