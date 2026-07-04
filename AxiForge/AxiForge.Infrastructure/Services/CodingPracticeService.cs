using System.Text.Json;
using AxiCore.Diagnostics;
using AxiForge.Application.DTOs.Coding;
using AxiForge.Application.Interfaces;
using AxiForge.Domain.Entities;
using AxiForge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AxiForge.Infrastructure.Services;

public sealed class CodingPracticeService : ICodingPracticeService
{
    private readonly AxiForgeDbContext _context;
    private readonly IJudge0Client _judge0Client;
    private readonly ILogger<CodingPracticeService> _logger;

    public CodingPracticeService(
        AxiForgeDbContext context,
        IJudge0Client judge0Client,
        ILogger<CodingPracticeService> logger)
    {
        _context = context;
        _judge0Client = judge0Client;
        _logger = logger;
    }

    /// <summary>
    /// Gets the published coding problem bank.
    /// Returns summary rows so the practice list can render quickly.
    /// </summary>
    public async Task<List<CodingProblemSummaryDto>> GetProblemsAsync(
        string? topic,
        string? difficulty,
        string? search,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(CodingPracticeService), nameof(GetProblemsAsync));
        try
        {
            var query = _context.CodingProblems
                .Where(x => x.IsPublished);

            if (!string.IsNullOrWhiteSpace(topic))
            {
                query = query.Where(x => x.Topic == topic || x.Tags.Contains(topic));
            }

            if (!string.IsNullOrWhiteSpace(difficulty))
            {
                query = query.Where(x => x.Difficulty == difficulty);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.Topic.Contains(search) ||
                    x.Tags.Contains(search));
            }

            return await query
                .OrderBy(x => x.Topic)
                .ThenBy(x => x.Title)
                .Select(x => new CodingProblemSummaryDto
                {
                    Id = x.Id,
                    Slug = x.Slug,
                    Title = x.Title,
                    Difficulty = x.Difficulty,
                    Topic = x.Topic,
                    Tags = x.Tags,
                    ClassTags = x.ClassTags,
                    CompanyTags = x.CompanyTags
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets one coding problem with visible sample test cases.
    /// Returns problem details so the editor can render instructions and starter code.
    /// </summary>
    public async Task<CodingProblemDetailDto?> GetProblemAsync(
        Guid problemId,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(CodingPracticeService), nameof(GetProblemAsync));
        try
        {
            var problem = await _context.CodingProblems
                .Include(x => x.TestCases)
                .FirstOrDefaultAsync(x => x.Id == problemId && x.IsPublished, cancellationToken);

            return problem == null ? null : ToDetail(problem);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates and evaluates a coding submission.
    /// Returns the stored submission result so the student can see pass/fail feedback.
    /// </summary>
    public async Task<CodingSubmissionDto> SubmitAsync(
        Guid accountId,
        CreateSubmissionRequestDto request,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(CodingPracticeService), nameof(SubmitAsync));
        try
        {
            var problem = await _context.CodingProblems
                .Include(x => x.TestCases)
                .FirstAsync(x => x.Id == request.ProblemId, cancellationToken);

            var testCases = problem.TestCases
                .OrderBy(x => x.Order)
                .Select(x => new CodingTestCaseDto
                {
                    Input = x.Input,
                    ExpectedOutput = x.ExpectedOutput
                })
                .ToList();

            var result = await _judge0Client.ExecuteAsync(request, testCases, cancellationToken);
            var submission = new CodingSubmission
            {
                AccountId = accountId,
                ProblemId = problem.Id,
                Language = request.Language,
                LanguageId = result.LanguageId,
                SourceCode = request.SourceCode,
                Status = result.Status,
                Output = result.Output,
                Error = result.Error,
                RuntimeMilliseconds = result.RuntimeMilliseconds,
                MemoryKb = result.MemoryKb,
                Judge0Tokens = result.Judge0Tokens,
                Judge0RawResult = result.Judge0RawResult,
                PassedTests = result.PassedTests,
                TotalTests = result.TotalTests,
                SubmittedAt = DateTime.UtcNow
            };

            _context.CodingSubmissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);

            return ToSubmissionDto(submission, problem.Title);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the authenticated student's submission history.
    /// Returns recent submissions so the student can review practice attempts.
    /// </summary>
    public async Task<List<CodingSubmissionDto>> GetMySubmissionsAsync(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(CodingPracticeService), nameof(GetMySubmissionsAsync));
        try
        {
            return await _context.CodingSubmissions
                .Include(x => x.Problem)
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.SubmittedAt)
                .Take(25)
                .Select(x => new CodingSubmissionDto
                {
                    Id = x.Id,
                    ProblemId = x.ProblemId,
                    ProblemTitle = x.Problem.Title,
                    Language = x.Language,
                    LanguageId = x.LanguageId,
                    Status = x.Status,
                    Output = x.Output,
                    Error = x.Error,
                    RuntimeMilliseconds = x.RuntimeMilliseconds,
                    MemoryKb = x.MemoryKb,
                    PassedTests = x.PassedTests,
                    TotalTests = x.TotalTests,
                    SubmittedAt = x.SubmittedAt
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Maps a coding problem entity to details.
    /// Returns a DTO because API clients should not depend on EF entities.
    /// </summary>
    private static CodingProblemDetailDto ToDetail(CodingProblem problem)
    {
        Console.WriteLine("Entering -> CodingPracticeService.cs -> ToDetail");
        try
        {
            return new CodingProblemDetailDto
            {
                Id = problem.Id,
                Slug = problem.Slug,
                Title = problem.Title,
                Difficulty = problem.Difficulty,
                Topic = problem.Topic,
                Tags = problem.Tags,
                ClassTags = problem.ClassTags,
                CompanyTags = problem.CompanyTags,
                Description = problem.Description,
                InputFormat = problem.InputFormat,
                OutputFormat = problem.OutputFormat,
                Constraints = problem.Constraints,
                Examples = problem.Examples,
                Explanation = problem.Explanation,
                StarterCode = problem.StarterCode,
                StarterCodeByLanguage = ParseStarterCodeByLanguage(problem.StarterCodeByLanguage, problem.StarterCode),
                TimeLimitMilliseconds = problem.TimeLimitMilliseconds,
                MemoryLimitKb = problem.MemoryLimitKb,
                SampleTestCases = problem.TestCases
                    .Where(x => !x.IsHidden)
                    .OrderBy(x => x.Order)
                    .Select(x => new CodingTestCaseDto
                    {
                        Input = x.Input,
                        ExpectedOutput = x.ExpectedOutput
                    })
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> CodingPracticeService.cs -> ToDetail -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> CodingPracticeService.cs -> ToDetail");
        }
    }

    /// <summary>
    /// Parses language-specific starter code from JSON.
    /// Returns a dictionary with a fallback local entry so the editor always has starter code.
    /// </summary>
    private static Dictionary<string, string> ParseStarterCodeByLanguage(string json, string fallbackStarterCode)
    {
        Console.WriteLine("Entering -> CodingPracticeService.cs -> ParseStarterCodeByLanguage");
        try
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed != null && parsed.Count > 0)
                {
                    return parsed;
                }
            }

            return new Dictionary<string, string>
            {
                ["local"] = fallbackStarterCode,
                ["csharp"] = fallbackStarterCode
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> CodingPracticeService.cs -> ParseStarterCodeByLanguage -> {ex.Message}");
            return new Dictionary<string, string>
            {
                ["local"] = fallbackStarterCode,
                ["csharp"] = fallbackStarterCode
            };
        }
        finally
        {
            Console.WriteLine("Exiting -> CodingPracticeService.cs -> ParseStarterCodeByLanguage");
        }
    }

    /// <summary>
    /// Maps a submission entity to a result DTO.
    /// Returns a DTO so API clients receive stable submission result data.
    /// </summary>
    private static CodingSubmissionDto ToSubmissionDto(CodingSubmission submission, string problemTitle)
    {
        Console.WriteLine("Entering -> CodingPracticeService.cs -> ToSubmissionDto");
        try
        {
            return new CodingSubmissionDto
            {
                Id = submission.Id,
                ProblemId = submission.ProblemId,
                ProblemTitle = problemTitle,
                Language = submission.Language,
                LanguageId = submission.LanguageId,
                Status = submission.Status,
                Output = submission.Output,
                Error = submission.Error,
                RuntimeMilliseconds = submission.RuntimeMilliseconds,
                MemoryKb = submission.MemoryKb,
                Judge0Tokens = submission.Judge0Tokens,
                Judge0RawResult = submission.Judge0RawResult,
                PassedTests = submission.PassedTests,
                TotalTests = submission.TotalTests,
                SubmittedAt = submission.SubmittedAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> CodingPracticeService.cs -> ToSubmissionDto -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> CodingPracticeService.cs -> ToSubmissionDto");
        }
    }
}
