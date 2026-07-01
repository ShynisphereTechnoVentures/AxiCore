namespace AxiForge.Application.DTOs.Coding;

public sealed class CreateSubmissionRequestDto
{
    public Guid ProblemId { get; set; }

    public string Language { get; set; } = "csharp";

    public string SourceCode { get; set; } = string.Empty;
}
