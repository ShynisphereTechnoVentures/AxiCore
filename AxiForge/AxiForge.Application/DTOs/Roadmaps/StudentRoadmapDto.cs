namespace AxiForge.Application.DTOs.Roadmaps;

public sealed class StudentRoadmapDto
{
    public Guid Id { get; set; }

    public Guid RoadmapTemplateId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int ProgressPercentage { get; set; }

    public DateTime EnrolledAt { get; set; }
}
