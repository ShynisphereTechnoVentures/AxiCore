namespace AxiForge.Domain.Entities;

public sealed class AssessmentTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DurationMinutes { get; set; } = 30;

    public int PassingScore { get; set; } = 70;

    public bool IsPublished { get; set; } = true;

    public List<AssessmentQuestion> Questions { get; set; } = new();
}
