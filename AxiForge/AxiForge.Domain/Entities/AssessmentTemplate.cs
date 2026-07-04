namespace AxiForge.Domain.Entities;

public sealed class AssessmentTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DurationMinutes { get; set; } = 30;

    public int PassingScore { get; set; } = 70;

    public string ClassTags { get; set; } = string.Empty;

    public string CompanyTags { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = true;

    public string ApprovalStatus { get; set; } = "Approved";

    public DateTime? SubmittedForApprovalAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string ApprovedBy { get; set; } = string.Empty;

    public string RejectionReason { get; set; } = string.Empty;

    public List<AssessmentQuestion> Questions { get; set; } = new();
}
