using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AxiCore.Contracts;
using AxiCore.Diagnostics;
using AxiCore.Security;
using AxiPlus.Application.DTOs.Practice;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AxiPlus.Infrastructure.Services;

public sealed class PracticeLaunchService : IPracticeLaunchService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly SignedTokenOptions _tokenOptions;
    private readonly ILogger<PracticeLaunchService> _logger;

    public PracticeLaunchService(
        AppDbContext context,
        IConfiguration configuration,
        IOptions<SignedTokenOptions> tokenOptions,
        ILogger<PracticeLaunchService> logger)
    {
        _context = context;
        _configuration = configuration;
        _tokenOptions = tokenOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Creates a signed AxiForge launch request for a specific AxiPlus lesson.
    /// Returns the redirect URL when the student has an active or trial entitlement because practice access is controlled by plan.
    /// </summary>
    public async Task<PracticeLaunchResponseDto> CreateLessonPracticeLaunchAsync(Guid studentUserId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(PracticeLaunchService), nameof(CreateLessonPracticeLaunchAsync));
        try
        {
            var student = await _context.Students
                .Include(x => x.Track)
                .FirstOrDefaultAsync(x => x.UserId == studentUserId, cancellationToken)
                ?? throw new InvalidOperationException("Student profile was not found.");

            var billing = await _context.StudentBillingAccounts
                .FirstOrDefaultAsync(x => x.StudentId == student.Id, cancellationToken);

            if (billing is null || (billing.Status != BillingStatus.Active && billing.Status != BillingStatus.Trial))
            {
                throw new UnauthorizedAccessException("AxiForge practice access requires an active plan entitlement.");
            }

            var lesson = await _context.Lessons
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x => x.Id == lessonId, cancellationToken)
                ?? throw new InvalidOperationException("Lesson was not found.");

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);
            var targetReference = string.IsNullOrWhiteSpace(lesson.PracticeLink)
                ? $"lesson:{lesson.Id}"
                : lesson.PracticeLink;

            var unsignedRequest = new PracticeLaunchRequest(
                SourceProduct: "AxiPlus",
                StudentId: student.Id,
                LessonId: lesson.Id,
                CourseId: null,
                PracticeType: "LessonCodingPractice",
                TargetReference: targetReference,
                ExpiresAt: expiresAt,
                Signature: string.Empty);

            var signature = CreateSignature(unsignedRequest);
            var launchRequest = unsignedRequest with { Signature = signature };
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(launchRequest)));
            var axiforgeBaseUrl = _configuration["Integrations:AxiForge:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:5290";

            return new PracticeLaunchResponseDto
            {
                LaunchRequest = launchRequest,
                RedirectUrl = $"{axiforgeBaseUrl}/practice/launch?token={Uri.EscapeDataString(encodedPayload)}"
            };
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Signs the practice launch request with the shared AxiCore signing key.
    /// Returns an HMAC signature so AxiForge can verify that the redirect was created by AxiPlus.
    /// </summary>
    private string CreateSignature(PracticeLaunchRequest request)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(PracticeLaunchService), nameof(CreateSignature));
        try
        {
            var signingKey = string.IsNullOrWhiteSpace(_tokenOptions.SigningKey)
                ? _configuration["Jwt:Key"]!
                : _tokenOptions.SigningKey;

            var payload = $"{request.SourceProduct}|{request.StudentId}|{request.LessonId}|{request.PracticeType}|{request.TargetReference}|{request.ExpiresAt:O}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingKey));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
