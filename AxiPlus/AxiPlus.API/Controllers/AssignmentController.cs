using System.Security.Claims;
using AxiPlus.Application.DTOs.Assignments;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize(Roles = "Student")]
[Route("api/[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IAssignmentService assignmentService)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AssignmentController.AssignmentController");
        _assignmentService = assignmentService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AssignmentController.GetMine");
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AssignmentController.IsNullOrWhiteSpace");
            return Unauthorized();
        }

        var assignments =
            await _assignmentService.GetForStudentAsync(email);

        return Ok(assignments);
    }

    [HttpPost("{assignmentId:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid assignmentId,
        SubmitAssignmentDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AssignmentController.Submit");
        if (string.IsNullOrWhiteSpace(dto.SubmissionLink))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AssignmentController.");
            return BadRequest("Submission link is required.");
        }

        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AssignmentController.");
            return Unauthorized();
        }

        var assignment = await _assignmentService.SubmitAsync(
            email,
            assignmentId,
            dto);

        return assignment == null
            ? NotFound()
            : Ok(assignment);
    }
}
