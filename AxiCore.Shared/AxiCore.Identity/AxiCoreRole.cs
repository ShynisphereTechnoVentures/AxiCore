namespace AxiCore.Identity;

public sealed class AxiCoreRole
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
}
