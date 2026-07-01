using System.Security.Claims;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize(Roles = "Student")]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{        
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AttendanceController.AttendanceController");
        _attendanceService = attendanceService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AttendanceController.GetMine");
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AttendanceController.GetMine.If");
            return Unauthorized();
        }

        var summary =
            await _attendanceService.GetForStudentAsync(email);

        return summary == null
            ? NotFound()
            : Ok(summary);
    }
}
