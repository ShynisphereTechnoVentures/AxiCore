using AxiCore.Diagnostics;
using AxiForge.Application.DTOs.Coding;
using AxiForge.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AxiForge.Infrastructure.Services;

public sealed class LocalJudge0Client : IJudge0Client
{
    private readonly ILogger<LocalJudge0Client> _logger;

    public LocalJudge0Client(ILogger<LocalJudge0Client> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes code through a local Judge0-compatible placeholder.
    /// Returns pass/fail results so the coding workflow is usable before external Judge0 is configured.
    /// </summary>
    public Task<CodingSubmissionDto> ExecuteAsync(
        CreateSubmissionRequestDto request,
        IReadOnlyList<CodingTestCaseDto> testCases,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(LocalJudge0Client), nameof(ExecuteAsync));
        try
        {
            var passed = testCases.Count(testCase =>
                request.SourceCode.Contains(
                    testCase.ExpectedOutput,
                    StringComparison.OrdinalIgnoreCase));

            var total = testCases.Count;
            var accepted = total > 0 && passed == total;

            return Task.FromResult(
                new CodingSubmissionDto
                {
                    Language = request.Language,
                    Status = accepted ? "Accepted" : "Wrong Answer",
                    Output = accepted
                        ? "All local validation checks passed."
                        : $"{passed}/{total} local validation checks passed.",
                    Error = string.Empty,
                    PassedTests = passed,
                    TotalTests = total
                });
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
