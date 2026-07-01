namespace AxiPlus.Domain.Entities;

public class TrackModule
{
    public int TrackId{ get; set; }

    public Track Track{ get; set; }
        = null!;

    public int ModuleId{ get; set; }

    public Module Module{ get; set; }
        = null!;

    public int Order{ get; set; }
}