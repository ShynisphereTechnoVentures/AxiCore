namespace AxiForge.Domain.Entities;

public sealed class RoadmapStep
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RoadmapTemplateId { get; set; }

    public RoadmapTemplate RoadmapTemplate { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }
}
