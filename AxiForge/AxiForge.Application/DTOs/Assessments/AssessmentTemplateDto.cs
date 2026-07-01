namespace AxiForge.Application.DTOs.Assessments;

public class AssessmentTemplateDto
{
    public Guid Id { get; set; }

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public int QuestionCount { get; set; }
}
