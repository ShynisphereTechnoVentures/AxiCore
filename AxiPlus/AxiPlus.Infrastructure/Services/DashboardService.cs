using AxiPlus.Application.DTOs.Dashboard;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class DashboardService : IDashboardService
{        
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<StudentDashboardDto>
        GetStudentDashboardAsync(Guid studentId)
   {
        var student = await _context.Students
           .Include(x => x.Track)
           .Include(x => x.Batch)
               .ThenInclude(x => x.Mentor)
           .Include(x => x.Batch)
               .ThenInclude(x => x.AssistantMentor)
           .FirstOrDefaultAsync(x => x.UserId == studentId);

        if (student == null)
       {
            return new StudentDashboardDto();
        }

        var studentModule = await _context.StudentModules
    .Include(x => x.Module)
    .Where(x => x.StudentId == student.Id &&
        x.Status == ModuleStatus.Active)
    .OrderByDescending(x => x.AssignedAt)
    .FirstOrDefaultAsync();

        var lessonProgress = await _context.StudentLessonProgresses
    .Include(x => x.Lesson)
    .Where(x => x.StudentId == student.Id)
    .OrderBy(x => x.Lesson.Order)
    .FirstOrDefaultAsync();

        var totalAttendance = await _context.Attendances
    .CountAsync(x => x.StudentId == student.Id);

        var presentAttendance = await _context.Attendances
            .CountAsync(x =>
                x.StudentId == student.Id &&
                x.Status == AttendanceStatus.Present);

        var attendancePercentage =
            totalAttendance == 0
                ? 0
                : (presentAttendance * 100) / totalAttendance;

        var totalLessons = await _context.Lessons.CountAsync();

        var completedLessons =
            await _context.StudentLessonProgresses
                .CountAsync(x =>
                    x.StudentId == student.Id &&
                    x.IsCompleted);

        var progressPercentage =
            totalLessons == 0
                ? 0
                : (completedLessons * 100) / totalLessons;

        var pendingAssignments = await _context.Assignments
            .Where(x =>
                x.BatchId == student.BatchId &&
                x.IsPublished)
            .CountAsync(x =>
                !_context.AssignmentSubmissions.Any(s =>
                    s.AssignmentId == x.Id &&
                    s.StudentId == student.Id));


        return new StudentDashboardDto
       {       
            StudentName = student.FullName,

            TrackName = student.Track.Name,

            BatchName = student.Batch.Name,

            MainMentor =
    student.Batch.Mentor?.FullName ?? "Not Assigned",

            AssistantMentor =
    student.Batch.AssistantMentor?.FullName ?? "Not Assigned",


            CurrentModule =
    studentModule?.Module?.Title
    ?? "No Module Assigned",

            CurrentLesson =
    lessonProgress?.Lesson?.Title
    ?? "No Lesson Assigned",


            AttendancePercentage = attendancePercentage,

            ProgressPercentage = progressPercentage,

            PendingAssignments = pendingAssignments



        };
    }
}
