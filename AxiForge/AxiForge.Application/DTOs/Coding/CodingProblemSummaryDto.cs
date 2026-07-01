namespace AxiForge.Application.DTOs.Coding;

public class CodingProblemSummaryDto
{
    public Guid Id { get; set; }

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;

    public string Topic { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;
}
