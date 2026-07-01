namespace AxiPlus.Domain.Entities;

public class ModuleResource
{
    public Guid Id{ get; set; }

    public int ModuleId{ get; set; }

    public Module Module{ get; set; }
        = null!;

    public string Name{ get; set; }
        = string.Empty;

    public string Url{ get; set; }
        = string.Empty;

    public string ResourceType{ get; set; }
        = string.Empty;
}