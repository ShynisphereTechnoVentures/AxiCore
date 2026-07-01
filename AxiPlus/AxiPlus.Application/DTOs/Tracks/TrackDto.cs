namespace AxiPlus.Application.DTOs.Tracks;

public class TrackDto
{
    public int Id{ get; set; }

    public string Name{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public bool IsActive{ get; set; }
    public string BatchPrefix{ get; set; } = string.Empty;

}
