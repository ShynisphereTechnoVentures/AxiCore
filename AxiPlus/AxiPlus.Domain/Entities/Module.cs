namespace AxiPlus.Domain.Entities;

public class Module
{
    public int Id{ get; set; }

    public string Title{ get; set; }
        = string.Empty;

    public string Description{ get; set; }
        = string.Empty;
    public bool IsPublished{ get; set; }

    public int Order{ get; set; }

    public bool IsActive{ get; set; }

    public DateTime CreatedAt{ get; set; }


    public ICollection<Lesson> Lessons{ get; set; }
    = new List<Lesson>();

    public ICollection<ModuleResource> Resources{ get; set; }
    = new List<ModuleResource>();

    public ICollection<TrackModule> TrackModules{ get; set; }
    = new List<TrackModule>();


}