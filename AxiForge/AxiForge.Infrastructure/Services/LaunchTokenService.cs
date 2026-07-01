using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AxiCore.Contracts;
using AxiCore.Diagnostics;
using AxiCore.Security;
using AxiForge.Application.DTOs.Launch;
using AxiForge.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AxiForge.Infrastructure.Services;

public sealed class LaunchTokenService : ILaunchTokenService
{
    private readonly SignedTokenOptions _tokenOptions;
    private readonly ILogger<LaunchTokenService> _logger;

    public LaunchTokenService(
        IOptions<SignedTokenOptions> tokenOptions,
        ILogger<LaunchTokenService> logger)
    {
        _tokenOptions = tokenOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Validates the first AxiPlus-to-AxiForge launch payload shape.
    /// Returns parsed context so AxiForge can route students to the correct future practice surface.
    /// </summary>
    public LaunchValidationResponseDto Validate(string token)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(LaunchTokenService), nameof(Validate));
        try
        {
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var request = JsonSerializer.Deserialize<PracticeLaunchRequest>(json);

            if (request == null ||
                request.ExpiresAt < DateTimeOffset.UtcNow ||
                !IsSignatureValid(request))
            {
                return new LaunchValidationResponseDto();
            }

            return new LaunchValidationResponseDto
            {
                IsValid = true,
                SourceProduct = request.SourceProduct,
                StudentId = request.StudentId.ToString(),
                LessonId = request.LessonId.ToString(),
                PracticeType = request.PracticeType,
                TargetReference = request.TargetReference
            };
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return new LaunchValidationResponseDto();
        }
    }

    /// <summary>
    /// Validates the AxiPlus launch request signature using the shared AxiCore signing key.
    /// Returns true only when the token was produced by a trusted product.
    /// </summary>
    private bool IsSignatureValid(PracticeLaunchRequest request)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(LaunchTokenService), nameof(IsSignatureValid));
        try
        {
            if (string.IsNullOrWhiteSpace(_tokenOptions.SigningKey) ||
                string.IsNullOrWhiteSpace(request.Signature))
            {
                return false;
            }

            var payload = $"{request.SourceProduct}|{request.StudentId}|{request.LessonId}|{request.PracticeType}|{request.TargetReference}|{request.ExpiresAt:O}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_tokenOptions.SigningKey));
            var expected = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(request.Signature));
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return false;
        }
    }
}
