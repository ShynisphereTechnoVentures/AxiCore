using AxiCore.Diagnostics;
using AxiForge.Application.DTOs.Assessments;
using AxiForge.Application.Interfaces;
using AxiForge.Domain.Entities;
using AxiForge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AxiForge.Infrastructure.Services;

public sealed class AssessmentService : IAssessmentService
{
    private readonly AxiForgeDbContext _context;
    private readonly ILogger<AssessmentService> _logger;

    public AssessmentService(AxiForgeDbContext context, ILogger<AssessmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets published assessment templates.
    /// Returns assessment cards for the student assessment list.
    /// </summary>
    public async Task<List<AssessmentTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AssessmentService), nameof(GetTemplatesAsync));
        try
        {
            return await _context.AssessmentTemplates
                .Include(x => x.Questions)
                .Where(x => x.IsPublished)
                .OrderBy(x => x.Title)
                .Select(x => new AssessmentTemplateDto
                {
                    Id = x.Id,
                    Slug = x.Slug,
                    Title = x.Title,
                    Description = x.Description,
                    DurationMinutes = x.DurationMinutes,
                    QuestionCount = x.Questions.Count
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
    /// Gets one assessment template with questions.
    /// Returns questions without correct answers so students can take the assessment.
    /// </summary>
    public async Task<AssessmentDetailDto?> GetTemplateAsync(Guid assessmentId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AssessmentService), nameof(GetTemplateAsync));
        try
        {
            var assessment = await _context.AssessmentTemplates
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == assessmentId && x.IsPublished, cancellationToken);

            return assessment == null ? null : ToDetail(assessment);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Submits an assessment attempt for scoring.
    /// Returns scored result so the student can see readiness feedback.
    /// </summary>
    public async Task<AssessmentResultDto> SubmitAsync(
        Guid accountId,
        SubmitAssessmentRequestDto request,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AssessmentService), nameof(SubmitAsync));
        try
        {
            var assessment = await _context.AssessmentTemplates
                .Include(x => x.Questions)
                .FirstAsync(x => x.Id == request.AssessmentTemplateId, cancellationToken);

            var answers = assessment.Questions
                .OrderBy(x => x.Order)
                .Select(question =>
                {
                    request.Answers.TryGetValue(question.Id, out var selected);
                    selected ??= string.Empty;
                    return new AssessmentAnswer
                    {
                        AssessmentQuestionId = question.Id,
                        SelectedOption = selected.Trim().ToUpperInvariant(),
                        IsCorrect = selected.Trim().Equals(question.CorrectOption, StringComparison.OrdinalIgnoreCase)
                    };
                })
                .ToList();

            var correct = answers.Count(x => x.IsCorrect);
            var total = assessment.Questions.Count;
            var score = total == 0 ? 0 : (correct * 100) / total;

            var attempt = new AssessmentAttempt
            {
                AccountId = accountId,
                AssessmentTemplateId = assessment.Id,
                SubmittedAt = DateTime.UtcNow,
                Score = score,
                Status = score >= assessment.PassingScore ? "Passed" : "Needs Practice",
                Answers = answers
            };

            _context.AssessmentAttempts.Add(attempt);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResult(attempt, assessment.Title, correct, total);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets previous assessment results for the authenticated student.
    /// Returns recent scores for readiness tracking.
    /// </summary>
    public async Task<List<AssessmentResultDto>> GetMyResultsAsync(Guid accountId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AssessmentService), nameof(GetMyResultsAsync));
        try
        {
            return await _context.AssessmentAttempts
                .Include(x => x.AssessmentTemplate)
                .Include(x => x.Answers)
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.StartedAt)
                .Take(20)
                .Select(x => new AssessmentResultDto
                {
                    AttemptId = x.Id,
                    Title = x.AssessmentTemplate.Title,
                    Score = x.Score,
                    CorrectAnswers = x.Answers.Count(a => a.IsCorrect),
                    TotalQuestions = x.Answers.Count,
                    Status = x.Status
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    private static AssessmentDetailDto ToDetail(AssessmentTemplate assessment)
    {
        Console.WriteLine("Entering -> AssessmentService.cs -> ToDetail");
        try
        {
            return new AssessmentDetailDto
            {
                Id = assessment.Id,
                Slug = assessment.Slug,
                Title = assessment.Title,
                Description = assessment.Description,
                DurationMinutes = assessment.DurationMinutes,
                QuestionCount = assessment.Questions.Count,
                Questions = assessment.Questions
                    .OrderBy(x => x.Order)
                    .Select(x => new AssessmentQuestionDto
                    {
                        Id = x.Id,
                        Prompt = x.Prompt,
                        OptionA = x.OptionA,
                        OptionB = x.OptionB,
                        OptionC = x.OptionC,
                        OptionD = x.OptionD,
                        Order = x.Order
                    })
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AssessmentService.cs -> ToDetail -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AssessmentService.cs -> ToDetail");
        }
    }

    private static AssessmentResultDto ToResult(
        AssessmentAttempt attempt,
        string title,
        int correct,
        int total)
    {
        Console.WriteLine("Entering -> AssessmentService.cs -> ToResult");
        try
        {
            return new AssessmentResultDto
            {
                AttemptId = attempt.Id,
                Title = title,
                Score = attempt.Score,
                CorrectAnswers = correct,
                TotalQuestions = total,
                Status = attempt.Status
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AssessmentService.cs -> ToResult -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AssessmentService.cs -> ToResult");
        }
    }
}
