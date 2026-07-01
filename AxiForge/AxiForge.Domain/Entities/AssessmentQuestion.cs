namespace AxiForge.Domain.Entities;

public sealed class AssessmentQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AssessmentTemplateId { get; set; }

    public AssessmentTemplate AssessmentTemplate { get; set; } = null!;

    public string Prompt { get; set; } = string.Empty;

    public string OptionA { get; set; } = string.Empty;

    public string OptionB { get; set; } = string.Empty;

    public string OptionC { get; set; } = string.Empty;

    public string OptionD { get; set; } = string.Empty;

    public string CorrectOption { get; set; } = "A";

    public int Order { get; set; }
}
