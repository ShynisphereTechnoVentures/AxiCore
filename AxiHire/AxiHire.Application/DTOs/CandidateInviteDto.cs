namespace AxiHire.Application.DTOs;

public sealed class CandidateInviteDto
{
    public Guid Id { get; set; }

    public string RecruiterOrganization { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime InvitedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}
