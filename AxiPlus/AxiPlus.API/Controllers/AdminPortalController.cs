using AxiPlus.Application.DTOs.AdminPortal;
using AxiPlus.Application.Interfaces;
using AxiCore.Identity;
using AxiPlus.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin-portal")]
public class AdminPortalController : ControllerBase
{        
    private readonly IAdminPortalService _adminPortalService;
    private readonly AxiCoreAccountProvisioningService _axiCoreProvisioningService;

    public AdminPortalController(
        IAdminPortalService adminPortalService,
        AxiCoreAccountProvisioningService axiCoreProvisioningService)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.AdminPortalController");
        _adminPortalService = adminPortalService;
        _axiCoreProvisioningService = axiCoreProvisioningService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetDashboard");
        return Ok(await _adminPortalService.GetDashboardAsync());
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetRoles");
        return Ok(await _adminPortalService.GetRolesAsync());
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetUsers");
        return Ok(await _adminPortalService.GetUsersAsync());
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(CreateAdminUserDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.CreateUser");
        if (string.IsNullOrWhiteSpace(dto.FullName) ||string.IsNullOrWhiteSpace(dto.Email) ||string.IsNullOrWhiteSpace(dto.Password))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.");
            return BadRequest("Name, email, and password are required.");
        }

        var user = await _adminPortalService.CreateUserAsync(dto);

        if (user?.Role == "Student")
        {
            await _axiCoreProvisioningService.ProvisionStudentAsync(
                dto.FullName,
                dto.Email,
                BCrypt.Net.BCrypt.HashPassword(dto.Password),
                new[] { AxiCoreProductCodes.AxiPlus, AxiCoreProductCodes.AxiForge },
                HttpContext.RequestAborted);
        }

        return user == null ? BadRequest("User could not be created.") : Ok(user);
    }

    [HttpPost("users/{userId:guid}/status")]
    public async Task<IActionResult> UpdateUserStatus(Guid userId,UpdateUserStatusDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.UpdateUserStatus");
        var user = await _adminPortalService.UpdateUserStatusAsync(userId, dto);

        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("tracks")]
    public async Task<IActionResult> GetTracks()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetTracks");
        return Ok(await _adminPortalService.GetTracksAsync());
    }

    [HttpPost("tracks")]
    public async Task<IActionResult> CreateTrack(CreateAdminTrackDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.CreateTrack");
        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Level) ||
            string.IsNullOrWhiteSpace(dto.BatchPrefix))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.");
            return BadRequest("Name, level, and batch prefix are required.");
        }

        var track = await _adminPortalService.CreateTrackAsync(dto);

        return track == null ? BadRequest("Track could not be created.") : Ok(track);
    }

    [HttpGet("batches")]
    public async Task<IActionResult> GetBatches()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetBatches");
        return Ok(await _adminPortalService.GetBatchesAsync());
    }

    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch(CreateAdminBatchDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.CreateBatch");
        if (string.IsNullOrWhiteSpace(dto.Name) ||
            dto.TrackId <= 0 ||
            dto.BatchNumber <= 0)
       {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.");
            return BadRequest("Name, track, and batch number are required.");
        }

        var batch = await _adminPortalService.CreateBatchAsync(dto);

        return batch == null ? BadRequest("Batch could not be created.") : Ok(batch);
    }

    [HttpPost("batches/{batchId:guid}/mentors")]
    public async Task<IActionResult> UpdateBatchMentors(
        Guid batchId,
        UpdateAdminBatchMentorsDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.UpdateBatchMentors");
        var batch = await _adminPortalService.UpdateBatchMentorsAsync(
            batchId,
            dto);

        return batch == null ? NotFound() : Ok(batch);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetStudents");
        return Ok(await _adminPortalService.GetStudentsAsync());
    }

    [HttpPost("students/{studentId:guid}/billing-status")]
    public async Task<IActionResult> UpdateStudentBillingStatus(
        Guid studentId,
        UpdateAdminStudentBillingStatusDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.UpdateStudentBillingStatus");
        var student = await _adminPortalService.UpdateStudentBillingStatusAsync(
            studentId,
            dto);

        return student == null ? NotFound() : Ok(student);
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModules()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetModules");
        return Ok(await _adminPortalService.GetModulesAsync());
    }

    [HttpPost("modules")]
    public async Task<IActionResult> CreateModule(CreateAdminModuleDto dto)
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.CreateModule");
        if (string.IsNullOrWhiteSpace(dto.Title))
       {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.");
            return BadRequest("Title is required.");
        }

        var module = await _adminPortalService.CreateModuleAsync(dto);

        return module == null ? BadRequest("Module could not be created.") : Ok(module);
    }

    [HttpGet("support-tickets")]
    public async Task<IActionResult> GetSupportTickets()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetSupportTickets");
        return Ok(await _adminPortalService.GetSupportTicketsAsync());
    }

    [HttpGet("assignment-submissions")]
    public async Task<IActionResult> GetAssignmentSubmissions()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetAssignmentSubmissions");
        return Ok(await _adminPortalService.GetAssignmentSubmissionsAsync());
    }

    [HttpGet("attendance-discrepancies")]
    public async Task<IActionResult> GetAttendanceDiscrepancies()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetAttendanceDiscrepancies");
        return Ok(await _adminPortalService.GetAttendanceDiscrepanciesAsync());
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetPayments");
        return Ok(await _adminPortalService.GetPaymentsAsync());
    }

    [HttpGet("batch-billing")]
    public async Task<IActionResult> GetBatchBilling()
   {        Console.WriteLine("Entered  -> AxiPlus.API.AdminPortalController.GetBatchBilling");
        return Ok(await _adminPortalService.GetBatchBillingAsync());
    }
}
