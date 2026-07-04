namespace AxiHire.Application.DTOs;

public sealed class CandidateDetailDto
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Headline { get; set; } = string.Empty;

    public string PrimarySkill { get; set; } = string.Empty;

    public string SkillSummary { get; set; } = string.Empty;

    public int ReadinessScore { get; set; }

    public string VerificationStatus { get; set; } = string.Empty;

    public DateTime LastSyncedAt { get; set; }

    public List<CandidateInviteDto> Invites { get; set; } = new();
}
