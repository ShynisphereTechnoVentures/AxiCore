using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.Modules;

public class StudentModuleModel
{
    public int ModuleId{get;set; }

    public string Title{get;set; }
        = string.Empty;

    public string Description{get;set; }
        = string.Empty;

    public int Order{get;set; }

    public int LessonCount{get;set; }

    public bool IsUnlocked{get;set; }

    public bool IsCompleted{get;set; }

    public decimal ProgressPercentage{get;set; }

    public ModuleStatus Status{get;set; }
}