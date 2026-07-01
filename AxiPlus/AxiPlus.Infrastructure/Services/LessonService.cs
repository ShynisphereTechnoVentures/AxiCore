using AxiPlus.Application.DTOs.Lessons;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class LessonService : ILessonService
{       
    private readonly AppDbContext _context;

    public LessonService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<List<LessonDto>> GetByModuleAsync(int moduleId)
   {
        return await _context.Lessons
            .Where(x => x.ModuleId == moduleId)
            .OrderBy(x => x.Order)
            .Select(x => new LessonDto
           {      
                Id = x.Id,
                ModuleId = x.ModuleId,
                Title = x.Title,
                Content = x.Content,
                Order = x.Order,
                PracticeLink = x.PracticeLink,
                IsPublished = x.IsPublished
            })
            .ToListAsync();
    }

    public async Task<LessonDto?> GetByIdAsync(Guid id)
   {
        return await _context.Lessons
            .Where(x => x.Id == id)
            .Select(x => new LessonDto
           {  
                Id = x.Id,
                ModuleId = x.ModuleId,
                Title = x.Title,
                Content = x.Content,
                Order = x.Order,
                PracticeLink = x.PracticeLink,
                IsPublished = x.IsPublished
            })
            .FirstOrDefaultAsync();
    }

    public async Task<LessonDto> CreateAsync(CreateLessonDto dto)
   {
        var lesson = new Lesson
       {        
            ModuleId = dto.ModuleId,
            Title = dto.Title,
            Content = dto.Content,
            Order = dto.Order,
            PracticeLink = dto.PracticeLink,
            IsPublished = dto.IsPublished
        };

        _context.Lessons.Add(lesson);

        await _context.SaveChangesAsync();

        return new LessonDto
       {       
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Content = lesson.Content,
            Order = lesson.Order,
            IsPublished = lesson.IsPublished
        };
    }

    public async Task<LessonDetailsDto> GetLessonDetailsAsync(Guid studentUserId,Guid lessonId)
   {
        var student = await _context.Students
            .FirstAsync(x => x.UserId == studentUserId);

        var lesson = await _context.Lessons
            .FirstAsync(x => x.Id == lessonId);

        var progress = await _context
            .StudentLessonProgresses
            .FirstOrDefaultAsync(x =>
                x.StudentId == student.Id &&
                x.LessonId == lessonId);

        return new LessonDetailsDto
       {       
            LessonId = lesson.Id,

            ModuleId = lesson.ModuleId,

            Title = lesson.Title,

            Description = lesson.Description,

            Content = lesson.Content,

            PracticeLink =
                lesson.PracticeLink ?? "",

            Status =
                progress?.Status
                ?? LessonStatus.Pending,

            IsCompleted =
                progress?.IsCompleted
                ?? false
        };
    }

    public async Task StartLessonAsync(Guid studentUserId, Guid lessonId)
   {
        var student = await _context.Students
            .FirstAsync(x => x.UserId == studentUserId);

        var progress = await GetOrCreateProgressAsync(student.Id, lessonId);

        if (progress.Status == LessonStatus.Pending)
       {
            progress.Status = LessonStatus.LiveSessionPending;
        }

        await _context.SaveChangesAsync();
    }

    public async Task CompleteLessonAsync(Guid studentUserId, Guid lessonId)
   {
        var student = await _context.Students
            .FirstAsync(x => x.UserId == studentUserId);

        var progress = await GetOrCreateProgressAsync(student.Id, lessonId);

        progress.Status = LessonStatus.Completed;
        progress.IsCompleted = true;

        await UpdateModuleProgressAsync(student.Id, lessonId);

        await _context.SaveChangesAsync();
    }

    private async Task<StudentLessonProgress> GetOrCreateProgressAsync(
        Guid studentId,
        Guid lessonId)
   {
        var progress = await _context.StudentLessonProgresses
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.LessonId == lessonId);

        if (progress != null)
       {
            return progress;
        }

        progress = new StudentLessonProgress
       {      
            Id = Guid.NewGuid(),
            StudentId = studentId,
            LessonId = lessonId,
            Status = LessonStatus.Pending,
            IsCompleted = false
        };

        _context.StudentLessonProgresses.Add(progress);

        return progress;
    }

    private async Task UpdateModuleProgressAsync(Guid studentId, Guid lessonId)
   {
        var lesson = await _context.Lessons
            .FirstAsync(x => x.Id == lessonId);

        var totalLessons = await _context.Lessons
            .CountAsync(x => x.ModuleId == lesson.ModuleId);

        if (totalLessons == 0)
       {
            return;
        }

        var completedLessons = await _context.StudentLessonProgresses
            .Where(x =>
                x.StudentId == studentId &&
                x.Lesson.ModuleId == lesson.ModuleId &&
                x.IsCompleted)
            .CountAsync();

        var studentModule = await _context.StudentModules
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.ModuleId == lesson.ModuleId);

        if (studentModule == null)
       {
            return;
        }

        studentModule.ProgressPercentage =
            Math.Round((decimal)completedLessons * 100 / totalLessons, 2);

        if (completedLessons >= totalLessons)
       {
            studentModule.IsCompleted = true;
            studentModule.Status = ModuleStatus.Completed;
            studentModule.CompletedAt ??= DateTime.UtcNow;
        }
        else if (studentModule.Status == ModuleStatus.Locked)
       {
            studentModule.Status = ModuleStatus.Active;
        }
    }
}
