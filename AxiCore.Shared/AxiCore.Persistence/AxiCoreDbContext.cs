using AxiCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AxiCore.Persistence;

public sealed class AxiCoreDbContext : DbContext
{
    public AxiCoreDbContext(DbContextOptions<AxiCoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<AxiCoreUser> Users => Set<AxiCoreUser>();

    public DbSet<AxiCoreRole> Roles => Set<AxiCoreRole>();

    public DbSet<AxiCoreUserRole> UserRoles => Set<AxiCoreUserRole>();

    public DbSet<AxiCoreProductAccess> UserProductAccess => Set<AxiCoreProductAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AxiCoreUser>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.FullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<AxiCoreRole>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<AxiCoreUserRole>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AxiCoreProductAccess>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.ProductCode }).IsUnique();
            entity.Property(x => x.ProductCode).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(80).IsRequired();
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
