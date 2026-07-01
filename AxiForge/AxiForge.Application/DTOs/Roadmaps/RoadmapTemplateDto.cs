namespace AxiForge.Application.DTOs.Roadmaps;

public class RoadmapTemplateDto
{
    public Guid Id { get; set; }

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public int StepCount { get; set; }
}
