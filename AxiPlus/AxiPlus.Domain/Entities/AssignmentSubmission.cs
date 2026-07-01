using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class AssignmentSubmission : BaseEntity
{
    public Guid AssignmentId{ get; set; }

    public Assignment Assignment{ get; set; } = null!;

    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public string SubmissionLink{ get; set; } = string.Empty;

    public string Notes{ get; set; } = string.Empty;

    public AssignmentSubmissionStatus Status{ get; set; }
        = AssignmentSubmissionStatus.Submitted;

    public DateTime SubmittedAt{ get; set; } = DateTime.UtcNow;

    public string Feedback{ get; set; } = string.Empty;
}
