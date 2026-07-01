namespace AxiForge.Application.DTOs.Assessments;

public sealed class AssessmentResultDto
{
    public Guid AttemptId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public string Status { get; set; } = string.Empty;
}
