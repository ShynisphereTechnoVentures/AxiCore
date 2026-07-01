namespace AxiPlus.Domain.Entities;

public class Track
{
    public int Id{ get; set; }

    public string Name{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public bool IsActive{ get; set; } = true;

    public DateTime CreatedAt{ get; set; }

    public string BatchPrefix{ get; set; }  = string.Empty;

    public ICollection<TrackModule> TrackModules{ get; set; }
    = new List<TrackModule>();
}