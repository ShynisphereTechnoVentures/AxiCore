namespace AxiForge.Application.DTOs.Coding;

public sealed class CodingSubmissionDto
{
    public Guid Id { get; set; }

    public Guid ProblemId { get; set; }

    public string ProblemTitle { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Output { get; set; } = string.Empty;

    public string Error { get; set; } = string.Empty;

    public int PassedTests { get; set; }

    public int TotalTests { get; set; }

    public DateTime SubmittedAt { get; set; }
}
