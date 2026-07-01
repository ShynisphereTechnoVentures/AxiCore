namespace AxiPlus.Domain.Entities;

public class Lesson
{
    public Guid Id{ get; set; }

    public int ModuleId{ get; set; }

    public Module Module{ get; set; }
        = null!;

    public string Title{ get; set; }
        = string.Empty;

    public string Content{ get; set; }
        = string.Empty;

    public int Order{ get; set; }

    public bool IsPublished{ get; set; }

    public DateTime CreatedAt{ get; set; }
        = DateTime.UtcNow;

    public ICollection<LessonLiveClass> LiveClasses{ get; set; }
= new List<LessonLiveClass>();

    public string PracticeLink{ get; set; }
        = string.Empty;

    public ICollection<StudentLessonProgress>
    Progresses
   { get; set; }
    = new List<StudentLessonProgress>();
    public string Description{ get; set; }
        = string.Empty;
}
