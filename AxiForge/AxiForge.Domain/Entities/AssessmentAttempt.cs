namespace AxiForge.Domain.Entities;

public sealed class AssessmentAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; }

    public Guid AssessmentTemplateId { get; set; }

    public AssessmentTemplate AssessmentTemplate { get; set; } = null!;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; set; }

    public int Score { get; set; }

    public string Status { get; set; } = "Started";

    public List<AssessmentAnswer> Answers { get; set; } = new();
}
