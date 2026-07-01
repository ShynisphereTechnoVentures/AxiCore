namespace AxiForge.Application.DTOs.Assessments;

public sealed class AssessmentDetailDto : AssessmentTemplateDto
{
    public List<AssessmentQuestionDto> Questions { get; set; } = new();
}
