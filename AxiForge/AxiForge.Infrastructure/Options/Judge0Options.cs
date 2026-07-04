namespace AxiForge.Infrastructure.Options;

public sealed class Judge0Options
{
    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiKeyHeaderName { get; set; } = "X-RapidAPI-Key";

    public int PollIntervalMilliseconds { get; set; } = 750;

    public int MaxPollAttempts { get; set; } = 20;

    public Dictionary<string, int> LanguageIds { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["csharp"] = 51,
        ["javascript"] = 63,
        ["python"] = 71,
        ["java"] = 62,
        ["cpp"] = 54
    };
}
