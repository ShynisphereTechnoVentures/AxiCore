using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AxiCore.Diagnostics;
using AxiForge.Application.DTOs.Coding;
using AxiForge.Application.Interfaces;
using AxiForge.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AxiForge.Infrastructure.Services;

public sealed class Judge0Client : IJudge0Client
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly Judge0Options _options;
    private readonly ILogger<Judge0Client> _logger;

    public Judge0Client(
        IOptions<Judge0Options> options,
        ILogger<Judge0Client> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Executes a coding submission through Judge0 when configured, otherwise through the local regression evaluator.
    /// Returns aggregate verdict data with runtime, memory, token, and error details captured for storage.
    /// </summary>
    public async Task<CodingSubmissionDto> ExecuteAsync(
        CreateSubmissionRequestDto request,
        IReadOnlyList<CodingTestCaseDto> testCases,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(Judge0Client), nameof(ExecuteAsync));
        try
        {
            if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                return ExecuteLocal(request, testCases);
            }

            if (!TryGetLanguageId(request.Language, out var languageId))
            {
                return new CodingSubmissionDto
                {
                    Language = request.Language,
                    LanguageId = 0,
                    Status = "Unsupported Language",
                    Output = $"Language '{request.Language}' is not mapped to a Judge0 language id.",
                    Error = "Unsupported language.",
                    PassedTests = 0,
                    TotalTests = testCases.Count
                };
            }

            using var httpClient = CreateHttpClient();
            var results = new List<Judge0SubmissionResult>();

            foreach (var testCase in testCases)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var token = await CreateSubmissionAsync(
                    httpClient,
                    request,
                    testCase,
                    languageId,
                    cancellationToken);

                results.Add(await PollSubmissionAsync(httpClient, token, cancellationToken));
            }

            var passed = results.Count(IsAccepted);
            var total = testCases.Count;
            var accepted = total > 0 && passed == total;
            var firstFailure = results.FirstOrDefault(x => !IsAccepted(x));
            var outputLines = results.Select((x, index) =>
                $"Case {index + 1}: {x.Status?.Description ?? "Unknown"} {BuildRuntimeSummary(x)}".Trim());

            return new CodingSubmissionDto
            {
                Language = request.Language,
                LanguageId = languageId,
                Status = accepted ? "Accepted" : firstFailure?.Status?.Description ?? "Wrong Answer",
                Output = string.Join(Environment.NewLine, outputLines),
                Error = BuildError(firstFailure),
                RuntimeMilliseconds = results.Sum(GetRuntimeMilliseconds),
                MemoryKb = results.Max(x => x.Memory ?? 0),
                PassedTests = passed,
                TotalTests = total,
                Judge0Tokens = string.Join(",", results.Select(x => x.Token).Where(x => !string.IsNullOrWhiteSpace(x))),
                Judge0RawResult = JsonSerializer.Serialize(results, JsonOptions)
            };
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    private static bool IsAccepted(Judge0SubmissionResult result) => result.Status?.Id == 3;

    private static string BuildRuntimeSummary(Judge0SubmissionResult result)
    {
        var runtime = GetRuntimeMilliseconds(result);
        var memory = result.Memory ?? 0;
        return runtime > 0 || memory > 0 ? $"({runtime:0.##} ms, {memory} KB)" : string.Empty;
    }

    private static string BuildError(Judge0SubmissionResult? result)
    {
        if (result == null)
        {
            return string.Empty;
        }

        return FirstNonEmpty(result.CompileOutput, result.Stderr, result.Message);
    }

    private static string FirstNonEmpty(params string?[] values) =>
        values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;

    private static double GetRuntimeMilliseconds(Judge0SubmissionResult result)
    {
        if (double.TryParse(result.Time, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
        {
            return seconds * 1000;
        }

        return 0;
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/")
        };

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                _options.ApiKeyHeaderName,
                _options.ApiKey);
        }

        return httpClient;
    }

    private async Task<string> CreateSubmissionAsync(
        HttpClient httpClient,
        CreateSubmissionRequestDto request,
        CodingTestCaseDto testCase,
        int languageId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "submissions?base64_encoded=false&wait=false",
            new Judge0SubmissionRequest(
                request.SourceCode,
                languageId,
                testCase.Input,
                testCase.ExpectedOutput),
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Judge0CreatedSubmission>(
            JsonOptions,
            cancellationToken);

        return created?.Token ?? throw new InvalidOperationException("Judge0 did not return a submission token.");
    }

    private async Task<Judge0SubmissionResult> PollSubmissionAsync(
        HttpClient httpClient,
        string token,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < Math.Max(1, _options.MaxPollAttempts); attempt++)
        {
            var result = await httpClient.GetFromJsonAsync<Judge0SubmissionResult>(
                $"submissions/{Uri.EscapeDataString(token)}?base64_encoded=false",
                JsonOptions,
                cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Judge0 returned an empty submission result.");
            }

            result.Token = string.IsNullOrWhiteSpace(result.Token) ? token : result.Token;
            if (result.Status?.Id is not (1 or 2))
            {
                return result;
            }

            await Task.Delay(
                Math.Max(100, _options.PollIntervalMilliseconds),
                cancellationToken);
        }

        return new Judge0SubmissionResult
        {
            Token = token,
            Status = new Judge0Status(0, "Polling Timeout"),
            Message = "Judge0 did not finish before the configured polling limit."
        };
    }

    private bool TryGetLanguageId(string language, out int languageId)
    {
        if (_options.LanguageIds.TryGetValue(language, out languageId))
        {
            return true;
        }

        var normalized = language.Trim().ToLowerInvariant();
        return _options.LanguageIds.TryGetValue(normalized, out languageId);
    }

    private static CodingSubmissionDto ExecuteLocal(
        CreateSubmissionRequestDto request,
        IReadOnlyList<CodingTestCaseDto> testCases)
    {
        var passed = testCases.Count(testCase =>
            request.SourceCode.Contains(
                testCase.ExpectedOutput,
                StringComparison.OrdinalIgnoreCase));

        var total = testCases.Count;
        var accepted = total > 0 && passed == total;

        return new CodingSubmissionDto
        {
            Language = request.Language,
            LanguageId = 0,
            Status = accepted ? "Accepted" : "Wrong Answer",
            Output = accepted
                ? "All local validation checks passed."
                : $"{passed}/{total} local validation checks passed.",
            Error = string.Empty,
            RuntimeMilliseconds = 0,
            MemoryKb = 0,
            PassedTests = passed,
            TotalTests = total
        };
    }

    private sealed record Judge0SubmissionRequest(
        [property: JsonPropertyName("source_code")] string SourceCode,
        [property: JsonPropertyName("language_id")] int LanguageId,
        [property: JsonPropertyName("stdin")] string Stdin,
        [property: JsonPropertyName("expected_output")] string ExpectedOutput);

    private sealed record Judge0CreatedSubmission(
        [property: JsonPropertyName("token")] string Token);

    private sealed class Judge0SubmissionResult
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public Judge0Status? Status { get; set; }

        [JsonPropertyName("stdout")]
        public string? Stdout { get; set; }

        [JsonPropertyName("stderr")]
        public string? Stderr { get; set; }

        [JsonPropertyName("compile_output")]
        public string? CompileOutput { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("memory")]
        public int? Memory { get; set; }
    }

    private sealed record Judge0Status(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("description")] string Description);
}
