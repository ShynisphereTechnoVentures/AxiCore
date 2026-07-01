namespace AxiForge.Domain.Entities;

public sealed class AssessmentAnswer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AssessmentAttemptId { get; set; }

    public AssessmentAttempt AssessmentAttempt { get; set; } = null!;

    public Guid AssessmentQuestionId { get; set; }

    public AssessmentQuestion AssessmentQuestion { get; set; } = null!;

    public string SelectedOption { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
