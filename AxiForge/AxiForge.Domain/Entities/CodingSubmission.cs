namespace AxiForge.Domain.Entities;

public sealed class CodingSubmission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; }

    public Guid ProblemId { get; set; }

    public CodingProblem Problem { get; set; } = null!;

    public string Language { get; set; } = "csharp";

    public string SourceCode { get; set; } = string.Empty;

    public string Status { get; set; } = "Queued";

    public string Output { get; set; } = string.Empty;

    public string Error { get; set; } = string.Empty;

    public int PassedTests { get; set; }

    public int TotalTests { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
