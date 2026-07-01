namespace AxiForge.Domain.Entities;

public sealed class RoadmapTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Level { get; set; } = "Beginner";

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<RoadmapStep> Steps { get; set; } = new();
}
