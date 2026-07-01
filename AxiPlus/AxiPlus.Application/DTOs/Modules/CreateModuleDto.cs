namespace AxiPlus.Application.DTOs.Modules;

public class CreateModuleDto
{
    public string Title{ get; set; }
        = string.Empty;

    public string Description{ get; set; }
        = string.Empty;

    public bool IsPublished{ get; set; }
}