namespace AxiCore.Identity;

public sealed class AxiCoreUserRole
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public AxiCoreUser User { get; set; } = null!;

    public Guid RoleId { get; set; }

    public AxiCoreRole Role { get; set; } = null!;
}
