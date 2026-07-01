namespace AxiPlus.Web.Models;

public class ModuleModel
{
    public int Id{get;set; }

    public string Title{get;set; }
        = string.Empty;

    public string Description{get;set; }
        = string.Empty;

    public bool IsPublished{get;set; }

    public int LessonCount{get;set; }

    public int Order{get;set; }
}