namespace AxiPlus.Application.DTOs.Progress;

public class LessonStatusDto
{
    public Guid LessonId{ get; set; }

    public string Status{ get; set; }
        = string.Empty;

    public string MentorRemarks{ get; set; }
        = string.Empty;

    public DateTime? ReviewedAt{ get; set; }
}