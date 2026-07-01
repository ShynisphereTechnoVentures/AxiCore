namespace AxiPlus.Application.DTOs.Assignments;

public class StudentAssignmentDto
{
    public Guid AssignmentId{ get; set; }

    public Guid? LessonId{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public string ModuleTitle{ get; set; } = string.Empty;

    public DateTime DueAt{ get; set; }

    public bool IsOverdue{ get; set; }

    public string Status{ get; set; } = string.Empty;

    public string SubmissionLink{ get; set; } = string.Empty;

    public DateTime? SubmittedAt{ get; set; }

    public string Feedback{ get; set; } = string.Empty;
}
