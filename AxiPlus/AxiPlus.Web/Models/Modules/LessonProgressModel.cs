using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.Modules;

public class LessonProgressModel
{
    public Guid LessonId{get;set; }

    public string Title{get;set; }
        = string.Empty;

    public string Description{get;set; }
        = string.Empty;

    public int Order{get;set; }

    public LessonStatus Status{get;set; }

    public bool IsCompleted{get;set; }
}