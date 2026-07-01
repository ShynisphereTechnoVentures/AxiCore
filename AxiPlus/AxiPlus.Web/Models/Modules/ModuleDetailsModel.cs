namespace AxiPlus.Web.Models.Modules;

public class ModuleDetailsModel
{
    public int ModuleId{get;set; }

    public string Title{get;set; }
        = string.Empty;

    public string Description{get;set; }
        = string.Empty;

    public decimal ProgressPercentage{get;set; }

    public List<LessonProgressModel> Lessons
   {get;set; } = new();
}