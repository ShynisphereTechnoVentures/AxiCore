using Microsoft.EntityFrameworkCore;
using AxiPlus.Application.DTOs.LessonLiveClasses;
using AxiPlus.Application.Interfaces;
using AxiPlus.Infrastructure.Data;

namespace AxiPlus.Infrastructure.Services;

public class LessonLiveClassService
    : ILessonLiveClassService
{   
    private readonly AppDbContext _context;

    public LessonLiveClassService(
        AppDbContext context)
   {
        _context = context;
    }

    public async Task<List<LessonLiveClassDto>>
        GetByLessonAsync(Guid lessonId)
   {
        return await _context.LessonLiveClasses
            .Where(x => x.LessonId == lessonId)
            .OrderBy(x => x.ScheduledAt)
            .Select(x => new LessonLiveClassDto
           {       
                Id = x.Id,
                LessonId = x.LessonId,
                MeetingLink = x.MeetingLink,
                RecordingLink = x.RecordingLink,
                ScheduledAt = x.ScheduledAt,
                IsCompleted = x.IsCompleted
            })
            .ToListAsync();
    }
}