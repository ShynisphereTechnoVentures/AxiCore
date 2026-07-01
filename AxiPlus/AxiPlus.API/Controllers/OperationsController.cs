using System.Reflection;
using System.Security.Claims;
using AxiPlus.Application.DTOs.Operations;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize]
[Route("api/operations")]
public class OperationsController : ControllerBase
{        
    private readonly IOperationsService _operationsService;

    public OperationsController(IOperationsService operationsService)
   {
        _operationsService = operationsService;
    }

    [HttpGet("mentor/profile")]
    [Authorize(Roles = "MainMentor,AssistantMentor")]
    public async Task<IActionResult> GetMentorProfile()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _operationsService.GetMentorProfileAsync(userId.Value));
    }

    [HttpGet("mentor/meeting-requests")]
    [Authorize(Roles = "MainMentor,AssistantMentor")]
    public async Task<IActionResult> GetMentorMeetingRequests()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _operationsService.GetMentorMeetingRequestsAsync(userId.Value));
    }

    [HttpPost("mentor/meeting-requests")]
    [Authorize(Roles = "MainMentor,AssistantMentor")]
    public async Task<IActionResult> CreateMeetingRequest(
        CreateMeetingRequestDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _operationsService.CreateMeetingRequestAsync(
            userId.Value,
            dto);

        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPost("mentor/meeting-requests/{meetingRequestId:guid}/follow-up")]
    [Authorize(Roles = "MainMentor,AssistantMentor")]
    public async Task<IActionResult> UpdateMeetingFollowUp(
        Guid meetingRequestId,
        UpdateMeetingFollowUpDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _operationsService.UpdateMeetingFollowUpAsync(
            userId.Value,
            meetingRequestId,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("mentor/attendance-discrepancies")]
    [Authorize(Roles = "MainMentor,AssistantMentor")]
    public async Task<IActionResult> GetMentorAttendanceDiscrepancies()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _operationsService.GetMentorAttendanceDiscrepanciesAsync(
                userId.Value));
    }

    [HttpPost("mentor/attendance-discrepancies/{discrepancyId:guid}/review")]
    [Authorize(Roles = "MainMentor,AssistantMentor")]
    public async Task<IActionResult> ReviewAttendanceDiscrepancy(
        Guid discrepancyId,
        ReviewAttendanceDiscrepancyDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _operationsService.ReviewAttendanceDiscrepancyAsync(
            userId.Value,
            discrepancyId,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("student/meeting-requests")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetStudentMeetingRequests()
   {        
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _operationsService.GetStudentMeetingRequestsAsync(
                userId.Value));
    }

    [HttpPost("student/meeting-requests/{meetingRequestId:guid}/respond")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RespondToMeetingRequest(
        Guid meetingRequestId,
        RespondMeetingRequestDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _operationsService.RespondToMeetingRequestAsync(
            userId.Value,
            meetingRequestId,
            dto);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("student/attendance-discrepancies")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetStudentAttendanceDiscrepancies()
   {
        var userId = GetUserId();

        return userId == null
            ? Unauthorized()
            : Ok(await _operationsService.GetStudentAttendanceDiscrepanciesAsync(
                userId.Value));
    }

    [HttpPost("student/attendance-discrepancies")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateAttendanceDiscrepancy(
        CreateAttendanceDiscrepancyDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _operationsService.CreateAttendanceDiscrepancyAsync(
            userId.Value,
            dto);

        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPost("admin/salary-slips")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateSalarySlip(CreateSalarySlipDto dto)
   {
        var userId = GetUserId();

        if (userId == null)
       {
            return Unauthorized();
        }

        var result = await _operationsService.CreateSalarySlipAsync(
            userId.Value,
            dto);

        return result == null ? BadRequest() : Ok(result);
    }

    private Guid? GetUserId()
   {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }
}
