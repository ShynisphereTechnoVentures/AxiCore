namespace AxiForge.Domain.Entities;

public sealed class StudentRoadmap
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; }

    public Guid RoadmapTemplateId { get; set; }

    public RoadmapTemplate RoadmapTemplate { get; set; } = null!;

    public int ProgressPercentage { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
