namespace AxiForge.Application.DTOs.Coding;

public sealed class CodingProblemDetailDto : CodingProblemSummaryDto
{
    public string Description { get; set; } = string.Empty;

    public string InputFormat { get; set; } = string.Empty;

    public string OutputFormat { get; set; } = string.Empty;

    public string Constraints { get; set; } = string.Empty;

    public string Examples { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string StarterCode { get; set; } = string.Empty;

    public Dictionary<string, string> StarterCodeByLanguage { get; set; } = new();

    public int TimeLimitMilliseconds { get; set; } = 1000;

    public int MemoryLimitKb { get; set; } = 262144;

    public List<CodingTestCaseDto> SampleTestCases { get; set; } = new();
}
