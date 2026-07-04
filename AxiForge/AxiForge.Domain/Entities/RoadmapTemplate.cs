namespace AxiForge.Domain.Entities;

public sealed class RoadmapTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Level { get; set; } = "Beginner";

    public string ClassTags { get; set; } = string.Empty;

    public string CompanyTags { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = true;

    public string ApprovalStatus { get; set; } = "Approved";

    public DateTime? SubmittedForApprovalAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string ApprovedBy { get; set; } = string.Empty;

    public string RejectionReason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<RoadmapStep> Steps { get; set; } = new();
}
