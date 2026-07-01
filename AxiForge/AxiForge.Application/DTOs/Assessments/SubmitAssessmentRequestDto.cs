namespace AxiForge.Application.DTOs.Assessments;

public sealed class SubmitAssessmentRequestDto
{
    public Guid AssessmentTemplateId { get; set; }

    public Dictionary<Guid, string> Answers { get; set; } = new();
}
