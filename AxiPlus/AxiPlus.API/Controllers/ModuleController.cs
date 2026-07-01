using System.Reflection;
using System.Security.Claims;
using AxiPlus.Application.DTOs.Modules;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModuleController : ControllerBase
{        
    private readonly IModuleService _moduleService;

    public ModuleController(IModuleService moduleService)
   {
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
   {
        var modules = await _moduleService.GetAllAsync();

        return Ok(modules);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
   {
        var module = await _moduleService.GetByIdAsync(id);

        if (module == null)
       {
            return NotFound();
        }

        return Ok(module);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateModuleDto dto)
   {
        var created = await _moduleService.CreateAsync(dto);

        return Ok(created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
   {
        var deleted = await _moduleService.DeleteAsync(id);

        if (!deleted)
       {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student/me")]
    public async Task<IActionResult> GetMyStudentModules()
   {
        var studentUserId = GetUserId();

        if (studentUserId == null)
       {        
            return Unauthorized();
        }

        var modules =
            await _moduleService
                .GetStudentModulesAsync(studentUserId.Value);

        return Ok(modules);
    }

    [Authorize]
    [HttpGet("student/{studentUserId:guid}")]
    public async Task<IActionResult> GetStudentModules(Guid studentUserId)
   {
        if (!CanAccessStudent(studentUserId))
       {
            return Forbid();
        }

        var modules =
            await _moduleService
                .GetStudentModulesAsync(studentUserId);

        return Ok(modules);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student/me/details/{moduleId:int}")]
    public async Task<IActionResult> GetMyDetails(int moduleId)
   {
        var studentUserId = GetUserId();

        if (studentUserId == null)
       {
            return Unauthorized();
        }

        var result =
            await _moduleService
                .GetModuleDetailsAsync(
                    studentUserId.Value,
                    moduleId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("student/{studentUserId:guid}/details/{moduleId:int}")]
    public async Task<IActionResult> GetDetails(
        Guid studentUserId,
        int moduleId)
   {
        if (!CanAccessStudent(studentUserId))
       {
            return Forbid();
        }

        var result =
            await _moduleService
                .GetModuleDetailsAsync(
                    studentUserId,
                    moduleId);

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
