namespace AxiPlus.Domain.Entities;

public class LessonLiveClass
{
    public Guid Id{ get; set; }

    public Guid LessonId{ get; set; }

    public Lesson Lesson{ get; set; }
        = null!;

    public string MeetingLink{ get; set; }
        = string.Empty;

    public string RecordingLink{ get; set; }
        = string.Empty;

    public DateTime ScheduledAt{ get; set; }

    public bool IsCompleted{ get; set; }

    public DateTime CreatedAt{ get; set; }
        = DateTime.UtcNow;
}