using System.Reflection;
using System.Security.Claims;
using AxiPlus.Application.DTOs.Lessons;
using AxiPlus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{        
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
   {
        _lessonService = lessonService;
    }

    [HttpGet("module/{moduleId:int}")]
    public async Task<IActionResult> GetByModule(int moduleId)
   {
        var lessons = await _lessonService.GetByModuleAsync(moduleId);

        return Ok(lessons);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
   {
        var lesson = await _lessonService.GetByIdAsync(id);

        if (lesson == null)
       {
            return NotFound();
        }

        return Ok(lesson);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonDto dto)
   {
        var lesson = await _lessonService.CreateAsync(dto);

        return Ok(lesson);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student/me/details/{lessonId:guid}")]
    public async Task<IActionResult> GetMyDetails(Guid lessonId)
   {
        var studentUserId = GetUserId();

        if (studentUserId == null)
       {
            return Unauthorized();
        }

        var result =
            await _lessonService
                .GetLessonDetailsAsync(
                    studentUserId.Value,
                    lessonId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("student/{studentUserId:guid}/details/{lessonId:guid}")]
    public async Task<IActionResult> GetDetails(
        Guid studentUserId,
        Guid lessonId)
   {
        if (!CanAccessStudent(studentUserId))
       {
            return Forbid();
        }

        var result =
            await _lessonService
                .GetLessonDetailsAsync(
                    studentUserId,
                    lessonId);

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpPost("student/me/start/{lessonId:guid}")]
    public async Task<IActionResult> StartMine(Guid lessonId)
   {
        var studentUserId = GetUserId();

        if (studentUserId == null)
       {
            return Unauthorized();
        }

        await _lessonService.StartLessonAsync(
            studentUserId.Value,
            lessonId);

        return NoContent();
    }

    [Authorize]
    [HttpPost("student/{studentUserId:guid}/start/{lessonId:guid}")]
    public async Task<IActionResult> Start(
        Guid studentUserId,
        Guid lessonId)
   {
        if (!CanAccessStudent(studentUserId))
       {
            return Forbid();
        }

        await _lessonService.StartLessonAsync(studentUserId, lessonId);

        return NoContent();
    }

    [Authorize(Roles = "Student")]
    [HttpPost("student/me/complete/{lessonId:guid}")]
    public async Task<IActionResult> CompleteMine(Guid lessonId)
   {
        var studentUserId = GetUserId();

        if (studentUserId == null)
       {
            return Unauthorized();
        }

        await _lessonService.CompleteLessonAsync(
            studentUserId.Value,
            lessonId);

        return NoContent();
    }

    [Authorize]
    [HttpPost("student/{studentUserId:guid}/complete/{lessonId:guid}")]
    public async Task<IActionResult> Complete(
        Guid studentUserId,
        Guid lessonId)
   {
        if (!CanAccessStudent(studentUserId))
       {
            return Forbid();
        }

        await _lessonService.CompleteLessonAsync(studentUserId, lessonId);

        return NoContent();
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
