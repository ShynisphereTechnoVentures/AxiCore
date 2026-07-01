namespace AxiForge.Application.DTOs.Launch;

public sealed class LaunchValidationResponseDto
{
    public bool IsValid { get; set; }

    public string SourceProduct { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string LessonId { get; set; } = string.Empty;

    public string PracticeType { get; set; } = string.Empty;

    public string TargetReference { get; set; } = string.Empty;
}
