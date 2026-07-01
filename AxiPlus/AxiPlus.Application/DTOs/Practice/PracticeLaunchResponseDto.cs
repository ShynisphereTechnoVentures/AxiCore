using AxiCore.Contracts;

namespace AxiPlus.Application.DTOs.Practice;

/// <summary>
/// Contains the AxiForge launch URL generated from an AxiPlus lesson.
/// Returns the signed launch request and redirect URL so the UI can send entitled students to the exact practice context.
/// </summary>
public sealed class PracticeLaunchResponseDto
{
    public string RedirectUrl{ get; set; } = string.Empty;

    public PracticeLaunchRequest LaunchRequest{ get; set; } = null!;
}
