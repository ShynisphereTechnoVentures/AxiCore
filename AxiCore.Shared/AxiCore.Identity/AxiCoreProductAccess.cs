namespace AxiCore.Identity;

public sealed class AxiCoreProductAccess
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public AxiCoreUser User { get; set; } = null!;

    public string ProductCode { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }
}
