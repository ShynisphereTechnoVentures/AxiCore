using AxiPlus.Application.DTOs.Modules;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class ModuleService : IModuleService
{        
    private readonly AppDbContext _context;

    public ModuleService(AppDbContext context)
   {
        _context = context;
    }


    public async Task<List<StudentModuleDto>> GetStudentModulesAsync(Guid studentUserId)
   {
        var student = await _context.Students
            .FirstAsync(x => x.UserId == studentUserId);

        return await _context.StudentModules
            .Include(x => x.Module)
            .Where(x => x.StudentId == student.Id)
            .OrderBy(x => x.Module.Order)
            .Select(x => new StudentModuleDto
           {        
                ModuleId = x.ModuleId,
                Title = x.Module.Title,
                Description = x.Module.Description,
                Order = x.Module.Order,
                IsUnlocked = x.IsUnlocked,
                IsCompleted = x.IsCompleted,
                ProgressPercentage = x.ProgressPercentage,
                Status = x.Status
            })
            .ToListAsync();
    }

    public async Task<List<ModuleDto>> GetAllAsync()
   {
        return await _context.Modules
            .Select(x => new ModuleDto
           {        
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                IsPublished = x.IsPublished,
                LessonCount = x.Lessons.Count,
                Order = x.Order
            })
            .ToListAsync();
    }

    public async Task<ModuleDto?> GetByIdAsync(int id)
   {
        return await _context.Modules
            .Where(x => x.Id == id)
            .Select(x => new ModuleDto
           {       
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                IsPublished = x.IsPublished,
                Order = x.Order,
                LessonCount = x.Lessons.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ModuleDto> CreateAsync(CreateModuleDto dto)
   {
        var module = new Module
       {       
            Title = dto.Title,
            Description = dto.Description,
            IsPublished = dto.IsPublished
        };

        _context.Modules.Add(module);

        await _context.SaveChangesAsync();

        return new ModuleDto
       {        
            Id = module.Id,
            Title = module.Title,
            Description = module.Description,
            IsPublished = module.IsPublished,
            Order = module.Order,
            LessonCount = 0
        };
    }

    public async Task<bool> DeleteAsync(int id)
   {
        var module = await _context.Modules
            .FirstOrDefaultAsync(x => x.Id == id);

        if (module == null)
            return false;

        _context.Modules.Remove(module);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ModuleDetailsDto>
    GetModuleDetailsAsync(
        Guid studentUserId,
        int moduleId)
   {
        var student = await _context.Students
            .FirstAsync(x => x.UserId == studentUserId);

        var studentModule = await _context.StudentModules
            .Include(x => x.Module)
            .FirstAsync(x =>
                x.StudentId == student.Id &&
                x.ModuleId == moduleId);

        var lessons = await _context.Lessons
            .Where(x => x.ModuleId == moduleId)
            .OrderBy(x => x.Order)
            .ToListAsync();

        var lessonProgress = await _context
            .StudentLessonProgresses
            .Where(x => x.StudentId == student.Id)
            .ToListAsync();

        return new ModuleDetailsDto
       {        
            ModuleId = studentModule.ModuleId,

            Title = studentModule.Module.Title,

            Description = studentModule.Module.Description,

            ProgressPercentage =
                studentModule.ProgressPercentage,

            Lessons = lessons.Select(lesson =>
           {
                var progress = lessonProgress
                    .FirstOrDefault(x =>
                        x.LessonId == lesson.Id);

                return new LessonProgressDto
               {        
                    LessonId = lesson.Id,

                    Title = lesson.Title,

                    Description =
                        lesson.Description,

                    Order = lesson.Order,

                    Status =
                        progress?.Status
                        ?? Domain.Enums.LessonStatus.Pending,

                    IsCompleted =
                        progress?.IsCompleted
                        ?? false
                };
            }).ToList()
        };
    }

}