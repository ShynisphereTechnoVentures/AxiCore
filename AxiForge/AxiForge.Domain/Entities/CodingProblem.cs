namespace AxiForge.Domain.Entities;

public sealed class CodingProblem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InputFormat { get; set; } = string.Empty;

    public string OutputFormat { get; set; } = string.Empty;

    public string Constraints { get; set; } = string.Empty;

    public string Examples { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Difficulty { get; set; } = "Easy";

    public string Topic { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

    public string StarterCode { get; set; } = string.Empty;

    public string StarterCodeByLanguage { get; set; } = string.Empty;

    public string ExpectedOutput { get; set; } = string.Empty;

    public int TimeLimitMilliseconds { get; set; } = 1000;

    public int MemoryLimitKb { get; set; } = 262144;

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CodingTestCase> TestCases { get; set; } = new();
}
