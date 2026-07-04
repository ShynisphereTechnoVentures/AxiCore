using AxiForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AxiForge.Infrastructure.Data;

public sealed class AxiForgeDbContext : DbContext
{
    public AxiForgeDbContext(DbContextOptions<AxiForgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<CodingProblem> CodingProblems => Set<CodingProblem>();

    public DbSet<CodingTestCase> CodingTestCases => Set<CodingTestCase>();

    public DbSet<CodingSubmission> CodingSubmissions => Set<CodingSubmission>();

    public DbSet<LessonPracticeSet> LessonPracticeSets => Set<LessonPracticeSet>();

    public DbSet<RoadmapTemplate> RoadmapTemplates => Set<RoadmapTemplate>();

    public DbSet<RoadmapStep> RoadmapSteps => Set<RoadmapStep>();

    public DbSet<StudentRoadmap> StudentRoadmaps => Set<StudentRoadmap>();

    public DbSet<AssessmentTemplate> AssessmentTemplates => Set<AssessmentTemplate>();

    public DbSet<AssessmentQuestion> AssessmentQuestions => Set<AssessmentQuestion>();

    public DbSet<AssessmentAttempt> AssessmentAttempts => Set<AssessmentAttempt>();

    public DbSet<AssessmentAnswer> AssessmentAnswers => Set<AssessmentAnswer>();

    public DbSet<AxiForgeAdminAuditEntry> AdminAuditEntries => Set<AxiForgeAdminAuditEntry>();

    public DbSet<AxiForgeTaxonomyItem> TaxonomyItems => Set<AxiForgeTaxonomyItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CodingProblem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Slug).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Difficulty).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Topic).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Tags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ClassTags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CompanyTags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ApprovalStatus).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ApprovedBy).HasMaxLength(256).IsRequired();
            entity.Property(x => x.TimeLimitMilliseconds).HasDefaultValue(1000);
            entity.Property(x => x.MemoryLimitKb).HasDefaultValue(262144);
            entity.HasMany(x => x.TestCases)
                .WithOne(x => x.Problem)
                .HasForeignKey(x => x.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CodingTestCase>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<CodingSubmission>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Language).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Judge0Tokens).IsRequired();
            entity.Property(x => x.Judge0RawResult).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.AccountId);
            entity.HasOne(x => x.Problem)
                .WithMany()
                .HasForeignKey(x => x.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LessonPracticeSet>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.SourceLessonId, x.ProblemId }).IsUnique();
            entity.Property(x => x.SourceProduct).HasMaxLength(80).IsRequired();
            entity.HasOne(x => x.Problem)
                .WithMany()
                .HasForeignKey(x => x.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RoadmapTemplate>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Slug).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Level).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ClassTags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CompanyTags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ApprovalStatus).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ApprovedBy).HasMaxLength(256).IsRequired();
            entity.HasMany(x => x.Steps)
                .WithOne(x => x.RoadmapTemplate)
                .HasForeignKey(x => x.RoadmapTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RoadmapStep>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(180).IsRequired();
        });

        modelBuilder.Entity<StudentRoadmap>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AccountId, x.RoadmapTemplateId }).IsUnique();
            entity.HasOne(x => x.RoadmapTemplate)
                .WithMany()
                .HasForeignKey(x => x.RoadmapTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AssessmentTemplate>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Slug).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(180).IsRequired();
            entity.Property(x => x.ClassTags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CompanyTags).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ApprovalStatus).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ApprovedBy).HasMaxLength(256).IsRequired();
            entity.HasMany(x => x.Questions)
                .WithOne(x => x.AssessmentTemplate)
                .HasForeignKey(x => x.AssessmentTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AssessmentQuestion>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CorrectOption).HasMaxLength(1).IsRequired();
        });

        modelBuilder.Entity<AssessmentAttempt>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.AccountId);
            entity.HasOne(x => x.AssessmentTemplate)
                .WithMany()
                .HasForeignKey(x => x.AssessmentTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Answers)
                .WithOne(x => x.AssessmentAttempt)
                .HasForeignKey(x => x.AssessmentAttemptId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AssessmentAnswer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SelectedOption).HasMaxLength(1).IsRequired();
            entity.HasOne(x => x.AssessmentQuestion)
                .WithMany()
                .HasForeignKey(x => x.AssessmentQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AxiForgeAdminAuditEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.EntityType, x.EntityId });
            entity.Property(x => x.EntityType).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Action).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Summary).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ActorEmail).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<AxiForgeTaxonomyItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.Type, x.Slug }).IsUnique();
            entity.Property(x => x.Type).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(180).IsRequired();
        });
    }
}
