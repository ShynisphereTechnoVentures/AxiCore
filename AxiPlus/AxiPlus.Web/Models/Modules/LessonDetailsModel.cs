using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.Modules;

public class LessonDetailsModel
{
    public Guid LessonId{get;set; }

    public string Title{get;set; }
        = string.Empty;

    public string Description{get;set; }
        = string.Empty;

    public string Content{get;set; }
        = string.Empty;

    public string PracticeLink{get;set; }
        = string.Empty;

    public LessonStatus Status{get;set; }

    public bool IsCompleted{get;set; }


    public int ModuleId{get;set; }


}