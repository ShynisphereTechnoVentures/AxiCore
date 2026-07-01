namespace AxiPlus.Application.DTOs.LessonLiveClasses;

public class LessonLiveClassDto
{
    public Guid Id{ get; set; }

    public Guid LessonId{ get; set; }

    public string MeetingLink{ get; set; }
        = string.Empty;

    public string RecordingLink{ get; set; }
        = string.Empty;

    public DateTime ScheduledAt{ get; set; }

    public bool IsCompleted{ get; set; }

    public string PracticeLink{ get; set; }
    = string.Empty;
}