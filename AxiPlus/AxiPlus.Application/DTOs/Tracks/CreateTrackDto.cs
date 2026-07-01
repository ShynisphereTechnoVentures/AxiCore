namespace AxiPlus.Application.DTOs.Tracks;

public class CreateTrackDto
{
    public string Name{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public string BatchPrefix{ get; set; }  = string.Empty;
}