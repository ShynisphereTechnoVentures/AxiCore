namespace AxiPlus.Application.DTOs.StudentPortal;

public class StudentRecordingDto
{
    public Guid LiveClassId{ get; set; }

    public Guid LessonId{ get; set; }

    public string LessonTitle{ get; set; } = string.Empty;

    public string ModuleTitle{ get; set; } = string.Empty;

    public string RecordingLink{ get; set; } = string.Empty;

    public DateTime ScheduledAt{ get; set; }
}
