namespace AxiHire.Domain.Entities;

public sealed class RecruiterOrganization
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
