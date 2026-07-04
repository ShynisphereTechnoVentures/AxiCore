namespace AxiHire.Domain.Entities;

public sealed class CandidateVerificationInvite
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CandidatePassportSnapshotId { get; set; }

    public CandidatePassportSnapshot CandidatePassportSnapshot { get; set; } = null!;

    public Guid RecruiterOrganizationId { get; set; }

    public RecruiterOrganization RecruiterOrganization { get; set; } = null!;

    public string Status { get; set; } = "Invited";

    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(14);
}
