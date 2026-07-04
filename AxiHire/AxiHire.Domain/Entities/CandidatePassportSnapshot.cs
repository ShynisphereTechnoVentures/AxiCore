namespace AxiHire.Domain.Entities;

public sealed class CandidatePassportSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AxiCoreUserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Headline { get; set; } = string.Empty;

    public string PrimarySkill { get; set; } = string.Empty;

    public string SkillSummary { get; set; } = string.Empty;

    public int ReadinessScore { get; set; }

    public string VerificationStatus { get; set; } = "Draft";

    public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
