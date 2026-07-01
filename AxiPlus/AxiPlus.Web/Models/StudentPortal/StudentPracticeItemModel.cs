using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.StudentPortal;

public class StudentPracticeItemModel
{
    public Guid LessonId{get;set; }

    public int ModuleId{get;set; }

    public string ModuleTitle{get;set; } = string.Empty;

    public string LessonTitle{get;set; } = string.Empty;

    public string PracticeLink{get;set; } = string.Empty;

    public LessonStatus Status{get;set; }

    public bool IsCompleted{get;set; }
}
