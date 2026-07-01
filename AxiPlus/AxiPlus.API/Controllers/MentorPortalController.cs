using System.Reflection;
using System.Security.Claims;
using AxiPlus.Application.DTOs.MentorPortal;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize(Roles = "MainMentor,AssistantMentor")]
[Route("api/mentor-portal")]
public class MentorPortalController : ControllerBase
{
    private readonly IMentorPortalService _mentorPortalService;

    public MentorPortalController(IMentorPortalService mentorPortalService)
   {
        _mentorPortalService = mentorPortalService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetDashboardAsync(userId.Value));
    }

    [HttpGet("batches")]
    public async Task<IActionResult> GetBatches()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetBatchesAsync(userId.Value));
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetStudentsAsync(userId.Value));
    }

    [HttpGet("lessons")]
    public async Task<IActionResult> GetLessonOptions()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetLessonOptionsAsync(userId.Value));
    }

    [HttpGet("live-classes")]
    public async Task<IActionResult> GetLiveClasses()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetLiveClassesAsync(userId.Value));
    }

    [HttpPost("live-classes")]
    public async Task<IActionResult> CreateLiveClass(
        CreateMentorLiveClassDto dto)
   {
        if (dto.LessonId == Guid.Empty ||
            string.IsNullOrWhiteSpace(dto.MeetingLink))
       {
            return BadRequest("Lesson and meeting link are required.");
        }

        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.CreateLiveClassAsync(
            userId.Value,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("live-classes/{liveClassId:guid}")]
    public async Task<IActionResult> DeleteLiveClass(Guid liveClassId)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var deleted = await _mentorPortalService.DeleteLiveClassAsync(
            userId.Value,
            liveClassId);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAssignments()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetAssignmentsAsync(userId.Value));
    }

    [HttpPost("assignments")]
    public async Task<IActionResult> CreateAssignment(
        CreateMentorAssignmentDto dto)
   {
        if (dto.BatchId == Guid.Empty ||
            string.IsNullOrWhiteSpace(dto.Title))
       {
            return BadRequest("Batch and title are required.");
        }

        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.CreateAssignmentAsync(
            userId.Value,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("assignments/{assignmentId:guid}")]
    public async Task<IActionResult> DeleteAssignment(Guid assignmentId)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var deleted = await _mentorPortalService.DeleteAssignmentAsync(
            userId.Value,
            assignmentId);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> GetSubmissions()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetSubmissionsAsync(userId.Value));
    }

    [HttpPost("submissions/{submissionId:guid}/review")]
    public async Task<IActionResult> ReviewSubmission(
        Guid submissionId,
        ReviewAssignmentSubmissionDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.ReviewSubmissionAsync(
            userId.Value,
            submissionId,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetSessionsAsync(userId.Value));
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession(
        CreateMentorSessionDto dto)
   {
        if (dto.BatchId == Guid.Empty ||
            string.IsNullOrWhiteSpace(dto.Title))
       {
            return BadRequest("Batch and title are required.");
        }

        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.CreateSessionAsync(
            userId.Value,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("weekly-sessions")]
    public async Task<IActionResult> CreateWeeklySessions(
        CreateMentorWeeklySessionsDto dto)
   {
        if (dto.BatchId == Guid.Empty ||
            string.IsNullOrWhiteSpace(dto.MeetLink) ||
            dto.Days.Count == 0)
       {
            return BadRequest("Batch, meeting link, and at least one day are required.");
        }

        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.CreateWeeklySessionsAsync(
            userId.Value,
            dto);

        return result.Count == 0 ? NotFound() : Ok(result);
    }

    [HttpDelete("sessions/{sessionId:guid}")]
    public async Task<IActionResult> DeleteSession(Guid sessionId)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var deleted = await _mentorPortalService.DeleteSessionAsync(
            userId.Value,
            sessionId);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("sessions/{sessionId:guid}/attendance")]
    public async Task<IActionResult> GetAttendanceRoster(Guid sessionId)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.GetAttendanceRosterAsync(
            userId.Value,
            sessionId);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("sessions/{sessionId:guid}/attendance")]
    public async Task<IActionResult> MarkAttendance(
        Guid sessionId,
        MarkMentorAttendanceDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.MarkAttendanceAsync(
            userId.Value,
            sessionId,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("student-reports")]
    public async Task<IActionResult> GetStudentReports()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetStudentReportsAsync(userId.Value));
    }

    [HttpGet("support-tickets")]
    public async Task<IActionResult> GetSupportTickets()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _mentorPortalService.GetSupportTicketsAsync(userId.Value));
    }

    [HttpPost("support-tickets/{ticketId:guid}/respond")]
    public async Task<IActionResult> RespondToSupportTicket(
        Guid ticketId,
        RespondSupportTicketDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _mentorPortalService.RespondToSupportTicketAsync(
            userId.Value,
            ticketId,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    private Guid? GetUserId()
   {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }
}
