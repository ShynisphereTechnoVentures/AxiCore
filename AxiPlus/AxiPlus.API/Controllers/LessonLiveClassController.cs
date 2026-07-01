using Microsoft.AspNetCore.Mvc;
using AxiPlus.Application.Interfaces;
using System.Reflection;

namespace AxiPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonLiveClassController
    : ControllerBase
{     
    private readonly ILessonLiveClassService
        _service;

    public LessonLiveClassController(
        ILessonLiveClassService service)
   {
        _service = service;
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<IActionResult>
        GetByLesson(Guid lessonId)
   {
        var result =
            await _service.GetByLessonAsync(
                lessonId);

        return Ok(result);
    }
}