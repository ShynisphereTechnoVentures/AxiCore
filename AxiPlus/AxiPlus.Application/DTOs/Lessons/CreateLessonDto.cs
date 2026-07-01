namespace AxiPlus.Application.DTOs.Lessons;

public class CreateLessonDto
{
    public int ModuleId{ get; set; }

    public string Title{ get; set; }
        = string.Empty;

    public string Content{ get; set; }
        = string.Empty;

    public int Order{ get; set; }

    public bool IsPublished{ get; set; }

    public string PracticeLink{ get; set; }
    = string.Empty;
}