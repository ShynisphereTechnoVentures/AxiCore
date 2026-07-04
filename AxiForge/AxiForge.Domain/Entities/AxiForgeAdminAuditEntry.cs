namespace AxiForge.Domain.Entities;

public sealed class AxiForgeAdminAuditEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EntityType { get; set; } = string.Empty;

    public Guid? EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string ActorEmail { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
