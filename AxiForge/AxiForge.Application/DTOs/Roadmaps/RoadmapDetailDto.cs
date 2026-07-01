namespace AxiForge.Application.DTOs.Roadmaps;

public sealed class RoadmapDetailDto : RoadmapTemplateDto
{
    public List<RoadmapStepDto> Steps { get; set; } = new();
}
