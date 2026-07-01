using System.Security.Claims;
using AxiCore.Diagnostics;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize]
[Route("api/practice-launch")]
public sealed class PracticeLaunchController : ControllerBase
{
    private readonly IPracticeLaunchService _practiceLaunchService;
    private readonly ILogger<PracticeLaunchController> _logger;

    public PracticeLaunchController(IPracticeLaunchService practiceLaunchService, ILogger<PracticeLaunchController> logger)
    {
        _practiceLaunchService = practiceLaunchService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a signed AxiForge launch URL for the authenticated student's lesson practice.
    /// Returns the redirect URL when the student has an active practice entitlement.
    /// </summary>
    [HttpPost("lessons/{lessonId:guid}")]
    public async Task<IActionResult> LaunchLessonPractice(Guid lessonId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(PracticeLaunchController), nameof(LaunchLessonPractice));
        try
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var studentUserId))
            {
                return Unauthorized("Invalid user context.");
            }

            var response = await _practiceLaunchService.CreateLessonPracticeLaunchAsync(studentUserId, lessonId, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            trace.Exception(ex);
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return Problem(ex.Message);
        }
    }
}
