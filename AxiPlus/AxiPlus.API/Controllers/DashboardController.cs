using System.Reflection;
using System.Security.Claims;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{       
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
   {
        _dashboardService = dashboardService;
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student/me")]
    public async Task<IActionResult> GetMyStudentDashboard()
   {
        var studentId = GetUserId();

        if (studentId == null)
       {
            return Unauthorized();
        }

        var result =
            await _dashboardService
                .GetStudentDashboardAsync(studentId.Value);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetStudentDashboard(Guid studentId)
   {
        if (!CanAccessStudent(studentId))
       {
            return Forbid();
        }

        var result =
            await _dashboardService
                .GetStudentDashboardAsync(studentId);

        return Ok(result);
    }

    private Guid? GetUserId()
   {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }

    private bool CanAccessStudent(Guid studentUserId)
   {
        var currentUserId = GetUserId();

        return currentUserId == studentUserId ||
            User.IsInRole("Admin") ||
            User.IsInRole("SuperAdmin") ||
            User.IsInRole("MainMentor") ||
            User.IsInRole("AssistantMentor");
    }
}
