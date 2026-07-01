namespace AxiForge.Application.DTOs.Assessments;

public sealed class AssessmentQuestionDto
{
    public Guid Id { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public string OptionA { get; set; } = string.Empty;

    public string OptionB { get; set; } = string.Empty;

    public string OptionC { get; set; } = string.Empty;

    public string OptionD { get; set; } = string.Empty;

    public int Order { get; set; }
}
