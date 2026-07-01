using AxiPlus.Application.DTOs.Assignments;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class AssignmentService : IAssignmentService
{       
    private readonly AppDbContext _context;

    public AssignmentService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<List<StudentAssignmentDto>> GetForStudentAsync(
        string email)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Email == email);

        if (student == null)
       {
            return new List<StudentAssignmentDto>();
        }

        return await _context.Assignments
            .Include(x => x.Lesson)
                .ThenInclude(x => x!.Module)
            .Include(x => x.Submissions
                .Where(s => s.StudentId == student.Id))
            .Where(x => x.BatchId == student.BatchId && x.IsPublished)
            .OrderBy(x => x.DueAt)
            .Select(x => new StudentAssignmentDto
           {
                AssignmentId = x.Id,
                LessonId = x.LessonId,
                Title = x.Title,
                Description = x.Description,
                ModuleTitle = x.Lesson != null
                    ? x.Lesson.Module.Title
                    : "General",
                DueAt = x.DueAt,
                IsOverdue = x.DueAt < DateTime.UtcNow &&
                    !x.Submissions.Any(),
                Status = x.Submissions.Any()
                    ? x.Submissions.First().Status.ToString()
                    : AssignmentSubmissionStatus.Pending.ToString(),
                SubmissionLink = x.Submissions.Any()
                    ? x.Submissions.First().SubmissionLink
                    : string.Empty,
                SubmittedAt = x.Submissions.Any()
                    ? x.Submissions.First().SubmittedAt
                    : null,
                Feedback = x.Submissions.Any()
                    ? x.Submissions.First().Feedback
                    : string.Empty
            })
            .ToListAsync();
    }

    public async Task<StudentAssignmentDto?> SubmitAsync(
        string email,
        Guid assignmentId,
        SubmitAssignmentDto dto)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Email == email);

        if (student == null)
       {
            return null;
        }

        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(x =>
                x.Id == assignmentId &&
                x.BatchId == student.BatchId &&
                x.IsPublished);

        if (assignment == null)
       {
            return null;
        }

        var submission = await _context.AssignmentSubmissions
            .FirstOrDefaultAsync(x =>
                x.AssignmentId == assignmentId &&
                x.StudentId == student.Id);

        if (submission == null)
       {
            submission = new AssignmentSubmission
           {
                AssignmentId = assignmentId,
                StudentId = student.Id
            };

            _context.AssignmentSubmissions.Add(submission);
        }

        submission.SubmissionLink = dto.SubmissionLink.Trim();
        submission.Notes = dto.Notes.Trim();
        submission.Status = AssignmentSubmissionStatus.Submitted;
        submission.SubmittedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var assignments = await GetForStudentAsync(email);

        return assignments.FirstOrDefault(x =>
            x.AssignmentId == assignmentId);
    }
}
