namespace AxiForge.Application.DTOs.Roadmaps;

public sealed class RoadmapStepDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }
}
