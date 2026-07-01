using AxiPlus.Application.DTOs.StudentPortal;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class StudentPortalService : IStudentPortalService
{      
    private readonly AppDbContext _context;

    public StudentPortalService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<List<StudentLiveClassDto>> GetLiveClassesAsync(
        string email)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return new List<StudentLiveClassDto>();
        }

        var attendance = await _context.Attendances
            .Where(x => x.StudentId == student.Id)
            .ToDictionaryAsync(x => x.SessionId, x => x.Status.ToString());

        var now = DateTime.UtcNow;

        return await _context.Sessions
            .Where(x => x.BatchId == student.BatchId)
            .OrderByDescending(x => x.StartTime)
            .Select(x => new StudentLiveClassDto
           {       
                SessionId = x.Id,
                Title = x.Title,
                MeetingLink = x.MeetLink,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Status = x.StartTime <= now && x.EndTime >= now
                    ? "Live"
                    : x.StartTime > now
                        ? "Upcoming"
                        : "Completed",
                AttendanceStatus = attendance.ContainsKey(x.Id)
                    ? attendance[x.Id]
                    : "NotMarked"
            })
            .ToListAsync();
    }

    public async Task<List<StudentRecordingDto>> GetRecordingsAsync(
        string email)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return new List<StudentRecordingDto>();
        }

        var moduleIds = await _context.StudentModules
            .Where(x => x.StudentId == student.Id && x.IsUnlocked)
            .Select(x => x.ModuleId)
            .ToListAsync();

        return await _context.LessonLiveClasses
            .Include(x => x.Lesson)
                .ThenInclude(x => x.Module)
            .Where(x =>
                x.IsCompleted &&
                x.RecordingLink != string.Empty &&
                moduleIds.Contains(x.Lesson.ModuleId))
            .OrderByDescending(x => x.ScheduledAt)
            .Select(x => new StudentRecordingDto
           {        
                LiveClassId = x.Id,
                LessonId = x.LessonId,
                LessonTitle = x.Lesson.Title,
                ModuleTitle = x.Lesson.Module.Title,
                RecordingLink = x.RecordingLink,
                ScheduledAt = x.ScheduledAt
            })
            .ToListAsync();
    }

    public async Task<List<StudentPracticeItemDto>> GetPracticeAsync(
        string email)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return new List<StudentPracticeItemDto>();
        }

        var unlockedModuleIds = await _context.StudentModules
            .Where(x => x.StudentId == student.Id && x.IsUnlocked)
            .Select(x => x.ModuleId)
            .ToListAsync();

        return await _context.Lessons
            .Include(x => x.Module)
            .Where(x =>
                unlockedModuleIds.Contains(x.ModuleId) &&
                x.PracticeLink != string.Empty)
            .OrderBy(x => x.Module.Order)
            .ThenBy(x => x.Order)
            .Select(x => new StudentPracticeItemDto
           {        
                LessonId = x.Id,
                ModuleId = x.ModuleId,
                ModuleTitle = x.Module.Title,
                LessonTitle = x.Title,
                PracticeLink = x.PracticeLink,
                Status = x.Progresses
                    .Where(p => p.StudentId == student.Id)
                    .Select(p => p.Status)
                    .FirstOrDefault(),
                IsCompleted = x.Progresses
                    .Any(p => p.StudentId == student.Id && p.IsCompleted)
            })
            .ToListAsync();
    }

    public async Task<List<StudentNotificationDto>> GetNotificationsAsync(
        string email)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return new List<StudentNotificationDto>();
        }

        return await _context.StudentNotifications
            .Where(x => x.StudentId == student.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StudentNotificationDto
           {    
                Id = x.Id,
                Title = x.Title,
                Message = x.Message,
                Type = x.Type,
                IsRead = x.IsRead,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> MarkNotificationReadAsync(
        string email,
        Guid notificationId)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return false;
        }

        var notification = await _context.StudentNotifications
            .FirstOrDefaultAsync(x =>
                x.Id == notificationId &&
                x.StudentId == student.Id);

        if (notification == null)
       {
            return false;
        }

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<SupportTicketDto>> GetSupportTicketsAsync(
        string email)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return new List<SupportTicketDto>();
        }

        return await _context.SupportTickets
            .Where(x => x.StudentId == student.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SupportTicketDto
           {    
                Id = x.Id,
                Subject = x.Subject,
                Message = x.Message,
                Status = x.Status,
                MentorResponse = x.MentorResponse,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SupportTicketDto?> CreateSupportTicketAsync(
        string email,
        CreateSupportTicketDto dto)
   {
        var student = await GetStudentAsync(email);

        if (student == null)
       {
            return null;
        }

        var ticket = new SupportTicket
       {       
            StudentId = student.Id,
            Subject = dto.Subject.Trim(),
            Message = dto.Message.Trim(),
            Status = SupportTicketStatus.Open
        };

        _context.SupportTickets.Add(ticket);

        await _context.SaveChangesAsync();

        return new SupportTicketDto
       {       
            Id = ticket.Id,
            Subject = ticket.Subject,
            Message = ticket.Message,
            Status = ticket.Status,
            MentorResponse = ticket.MentorResponse,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }

    private async Task<Student?> GetStudentAsync(string email)
   {
        return await _context.Students
            .FirstOrDefaultAsync(x => x.Email == email);
    }
}
