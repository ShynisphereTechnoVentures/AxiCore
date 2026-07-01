using System.Reflection;
using System.Security.Claims;
using AxiPlus.Application.DTOs.StudentPortal;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize(Roles = "Student")]
[Route("api/student-portal")]
public class StudentPortalController : ControllerBase
{
    private readonly IStudentPortalService _studentPortalService;

    public StudentPortalController(
        IStudentPortalService studentPortalService)
   {
        _studentPortalService = studentPortalService;
    }

    [HttpGet("live-classes")]
    public async Task<IActionResult> GetLiveClasses()
   {
        var email = GetEmail();

        return string.IsNullOrWhiteSpace(email)
            ? Unauthorized()
            : Ok(await _studentPortalService.GetLiveClassesAsync(email));
    }

    [HttpGet("recordings")]
    public async Task<IActionResult> GetRecordings()
   {
        var email = GetEmail();

        return string.IsNullOrWhiteSpace(email)
            ? Unauthorized()
            : Ok(await _studentPortalService.GetRecordingsAsync(email));
    }

    [HttpGet("practice")]
    public async Task<IActionResult> GetPractice()
   {
        var email = GetEmail();

        return string.IsNullOrWhiteSpace(email)
            ? Unauthorized()
            : Ok(await _studentPortalService.GetPracticeAsync(email));
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
   {
        var email = GetEmail();

        return string.IsNullOrWhiteSpace(email)
            ? Unauthorized()
            : Ok(await _studentPortalService.GetNotificationsAsync(email));
    }

    [HttpPost("notifications/{id:guid}/read")]
    public async Task<IActionResult> MarkNotificationRead(Guid id)
   {
        var email = GetEmail();

        if (string.IsNullOrWhiteSpace(email))
       {
            return Unauthorized();
        }

        var updated = await _studentPortalService
            .MarkNotificationReadAsync(email, id);

        return updated ? NoContent() : NotFound();
    }

    [HttpGet("support-tickets")]
    public async Task<IActionResult> GetSupportTickets()
   {
        var email = GetEmail();

        return string.IsNullOrWhiteSpace(email)
            ? Unauthorized()
            : Ok(await _studentPortalService.GetSupportTicketsAsync(email));
    }

    [HttpPost("support-tickets")]
    public async Task<IActionResult> CreateSupportTicket(
        CreateSupportTicketDto dto)
   {
        if (string.IsNullOrWhiteSpace(dto.Subject) ||
            string.IsNullOrWhiteSpace(dto.Message))
       {
            return BadRequest("Subject and message are required.");
        }

        var email = GetEmail();

        if (string.IsNullOrWhiteSpace(email))
       {
            return Unauthorized();
        }

        var ticket = await _studentPortalService
            .CreateSupportTicketAsync(email, dto);

        return ticket == null ? NotFound() : Ok(ticket);
    }

    private string? GetEmail()
   {
        return User.FindFirstValue(ClaimTypes.Email);
    }
}
