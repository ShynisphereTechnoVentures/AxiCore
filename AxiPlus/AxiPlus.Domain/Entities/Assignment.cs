using AxiPlus.Domain.Common;

namespace AxiPlus.Domain.Entities;

public class Assignment : BaseEntity
{
    public Guid BatchId{ get; set; }

    public Batch Batch{ get; set; } = null!;

    public Guid? LessonId{ get; set; }

    public Lesson? Lesson{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public DateTime DueAt{ get; set; }

    public bool IsPublished{ get; set; } = true;

    public Guid? CreatedByUserId{ get; set; }

    public User? CreatedByUser{ get; set; }

    public Guid? AssignedMentorUserId{ get; set; }

    public User? AssignedMentorUser{ get; set; }

    public string SourceRole{ get; set; } = string.Empty;

    public ICollection<AssignmentSubmission> Submissions{ get; set; }
        = new List<AssignmentSubmission>();
}
