using AxiPlus.Application.DTOs.MentorPortal;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class MentorPortalService : IMentorPortalService
{
    private readonly AppDbContext _context;

    public MentorPortalService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<MentorDashboardDto?> GetDashboardAsync(Guid mentorUserId)
   {
        var mentor = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == mentorUserId);

        if (mentor == null)
       {
            return null;
        }

        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);
        var lessonIds = await GetMentorLessonIdsAsync(mentorUserId);
        var now = DateTime.UtcNow;

        var studentCount = await _context.Students
            .CountAsync(x => batchIds.Contains(x.BatchId));

        var upcomingClasses = await _context.LessonLiveClasses
            .Where(x =>
                lessonIds.Contains(x.LessonId) &&
                !x.IsCompleted &&
                x.ScheduledAt >= now)
            .CountAsync();

        var pendingSubmissions = await _context.AssignmentSubmissions
            .CountAsync(x =>
                batchIds.Contains(x.Assignment.BatchId) &&
                x.Status == AssignmentSubmissionStatus.Submitted);

        var openTickets = await _context.SupportTickets
            .CountAsync(x =>
                batchIds.Contains(x.Student.BatchId) &&
                x.Status != SupportTicketStatus.Resolved);

        var sessions = await _context.Sessions
            .CountAsync(x => batchIds.Contains(x.BatchId));

        return new MentorDashboardDto
       {
            MentorName = mentor.FullName,
            Role = mentor.Role.Name,
            AssignedBatchCount = batchIds.Count,
            StudentCount = studentCount,
            UpcomingClassCount = upcomingClasses,
            PendingSubmissionCount = pendingSubmissions,
            OpenSupportTicketCount = openTickets,
            AttendanceSessionCount = sessions
        };
    }

    public async Task<List<MentorBatchDto>> GetBatchesAsync(Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var batches = await _context.Batches
            .Include(x => x.Track)
            .Where(x => batchIds.Contains(x.Id))
            .OrderBy(x => x.Name)
            .ToListAsync();

        var result = new List<MentorBatchDto>();

        foreach (var batch in batches)
       {
            var studentIds = await _context.Students
                .Where(x => x.BatchId == batch.Id)
                .Select(x => x.Id)
                .ToListAsync();

            var averageProgress = studentIds.Count == 0
                ? 0
                : await _context.StudentModules
                    .Where(x => studentIds.Contains(x.StudentId))
                    .AverageAsync(x => (decimal?)x.ProgressPercentage) ?? 0;

            var attendance = await GetAttendancePercentageAsync(studentIds);

            result.Add(new MentorBatchDto
           {
                BatchId = batch.Id,
                Name = batch.Name,
                TrackName = batch.Track.Name,
                Level = batch.Level,
                CurrentStrength = studentIds.Count,
                Capacity = batch.Capacity,
                AverageProgressPercentage = Math.Round(averageProgress, 2),
                AttendancePercentage = attendance
            });
        }

        return result;
    }

    public async Task<List<MentorStudentDto>> GetStudentsAsync(Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var students = await _context.Students
            .Include(x => x.Batch)
            .Include(x => x.Track)
            .Where(x => batchIds.Contains(x.BatchId))
            .OrderBy(x => x.Batch.Name)
            .ThenBy(x => x.FullName)
            .ToListAsync();

        var totalLessons = await _context.Lessons.CountAsync();
        var result = new List<MentorStudentDto>();

        foreach (var student in students)
       {
            var attendance = await GetAttendancePercentageAsync(
                new List<Guid>{ student.Id });

            var progress = await _context.StudentModules
                .Where(x => x.StudentId == student.Id)
                .AverageAsync(x => (decimal?)x.ProgressPercentage) ?? 0;

            var completedLessons = await _context.StudentLessonProgresses
                .CountAsync(x =>
                    x.StudentId == student.Id &&
                    x.IsCompleted);

            var pendingAssignments = await _context.Assignments
                .Where(x =>
                    x.BatchId == student.BatchId &&
                    x.IsPublished)
                .CountAsync(x =>
                    !_context.AssignmentSubmissions.Any(s =>
                        s.AssignmentId == x.Id &&
                        s.StudentId == student.Id &&
                        s.Status == AssignmentSubmissionStatus.Reviewed));

            result.Add(new MentorStudentDto
           {
                StudentId = student.Id,
                UserId = student.UserId,
                BatchId = student.BatchId,
                FullName = student.FullName,
                Email = student.Email,
                BatchName = student.Batch.Name,
                TrackName = student.Track.Name,
                AttendancePercentage = attendance,
                ProgressPercentage = Math.Round(progress, 2),
                PendingAssignments = pendingAssignments,
                CompletedLessons = completedLessons,
                TotalLessons = totalLessons
            });
        }

        return result;
    }

    public async Task<List<MentorLessonOptionDto>> GetLessonOptionsAsync(
        Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var trackIds = await _context.Batches
            .Where(x => batchIds.Contains(x.Id))
            .Select(x => x.TrackId)
            .Distinct()
            .ToListAsync();

        var moduleIds = await _context.TrackModules
            .Where(x => trackIds.Contains(x.TrackId))
            .Select(x => x.ModuleId)
            .Distinct()
            .ToListAsync();

        return await _context.Lessons
            .Include(x => x.Module)
            .Where(x => moduleIds.Contains(x.ModuleId))
            .OrderBy(x => x.Module.Order)
            .ThenBy(x => x.Order)
            .Select(x => new MentorLessonOptionDto
           {
                LessonId = x.Id,
                ModuleId = x.ModuleId,
                ModuleTitle = x.Module.Title,
                LessonTitle = x.Title
            })
            .ToListAsync();
    }

    public async Task<List<MentorLiveClassDto>> GetLiveClassesAsync(
        Guid mentorUserId)
   {
        var lessonIds = await GetMentorLessonIdsAsync(mentorUserId);

        return await _context.LessonLiveClasses
            .Include(x => x.Lesson)
                .ThenInclude(x => x.Module)
            .Where(x => lessonIds.Contains(x.LessonId))
            .OrderByDescending(x => x.ScheduledAt)
            .Select(x => new MentorLiveClassDto
           {
                Id = x.Id,
                LessonId = x.LessonId,
                LessonTitle = x.Lesson.Title,
                ModuleTitle = x.Lesson.Module.Title,
                ScheduledAt = x.ScheduledAt,
                MeetingLink = x.MeetingLink,
                RecordingLink = x.RecordingLink,
                IsCompleted = x.IsCompleted
            })
            .ToListAsync();
    }

    public async Task<MentorLiveClassDto?> CreateLiveClassAsync(
        Guid mentorUserId,
        CreateMentorLiveClassDto dto)
   {
        var lessonIds = await GetMentorLessonIdsAsync(mentorUserId);

        if (!lessonIds.Contains(dto.LessonId))
       {
            return null;
        }

        var liveClass = new LessonLiveClass
       {
            Id = Guid.NewGuid(),
            LessonId = dto.LessonId,
            ScheduledAt = DateTime.SpecifyKind(dto.ScheduledAt, DateTimeKind.Utc),
            MeetingLink = dto.MeetingLink.Trim(),
            RecordingLink = dto.RecordingLink.Trim(),
            IsCompleted = false
        };

        _context.LessonLiveClasses.Add(liveClass);

        await _context.SaveChangesAsync();

        return (await GetLiveClassesAsync(mentorUserId))
            .FirstOrDefault(x => x.Id == liveClass.Id);
    }

    public async Task<bool> DeleteLiveClassAsync(
        Guid mentorUserId,
        Guid liveClassId)
   {
        var lessonIds = await GetMentorLessonIdsAsync(mentorUserId);

        var liveClass = await _context.LessonLiveClasses
            .FirstOrDefaultAsync(x =>
                x.Id == liveClassId &&
                lessonIds.Contains(x.LessonId));

        if (liveClass == null)
       {
            return false;
        }

        _context.LessonLiveClasses.Remove(liveClass);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<MentorAssignmentDto>> GetAssignmentsAsync(
        Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        return await _context.Assignments
            .Include(x => x.Batch)
            .Include(x => x.Lesson)
                .ThenInclude(x => x!.Module)
            .Include(x => x.CreatedByUser)
            .Include(x => x.Submissions)
            .Where(x => batchIds.Contains(x.BatchId))
            .OrderByDescending(x => x.DueAt)
            .Select(x => new MentorAssignmentDto
           {
                AssignmentId = x.Id,
                BatchId = x.BatchId,
                BatchName = x.Batch.Name,
                LessonId = x.LessonId,
                LessonTitle = x.Lesson != null ? x.Lesson.Title : "General",
                ModuleTitle = x.Lesson != null ? x.Lesson.Module.Title : "General",
                Title = x.Title,
                Description = x.Description,
                DueAt = x.DueAt,
                IsPublished = x.IsPublished,
                IsOverdue = x.DueAt < DateTime.UtcNow,
                CreatedByName = x.CreatedByUser != null
                    ? x.CreatedByUser.FullName
                    : "System",
                SourceRole = x.SourceRole,
                SubmissionCount = x.Submissions.Count,
                PendingReviewCount = x.Submissions.Count(s =>
                    s.Status == AssignmentSubmissionStatus.Submitted)
            })
            .ToListAsync();
    }

    public async Task<MentorAssignmentDto?> CreateAssignmentAsync(
        Guid mentorUserId,
        CreateMentorAssignmentDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        if (!batchIds.Contains(dto.BatchId))
       {
            return null;
        }

        if (dto.LessonId.HasValue)
       {
            var lessonIds = await GetMentorLessonIdsAsync(mentorUserId);

            if (!lessonIds.Contains(dto.LessonId.Value))
           {
                return null;
            }
        }

        var mentor = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == mentorUserId);

        var assignment = new Assignment
       {
            BatchId = dto.BatchId,
            LessonId = dto.LessonId,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            DueAt = DateTime.SpecifyKind(dto.DueAt, DateTimeKind.Utc),
            IsPublished = dto.IsPublished,
            CreatedByUserId = mentorUserId,
            AssignedMentorUserId = mentorUserId,
            SourceRole = mentor?.Role.Name ?? string.Empty
        };

        _context.Assignments.Add(assignment);

        await _context.SaveChangesAsync();

        return (await GetAssignmentsAsync(mentorUserId))
            .FirstOrDefault(x => x.AssignmentId == assignment.Id);
    }

    public async Task<bool> DeleteAssignmentAsync(
        Guid mentorUserId,
        Guid assignmentId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(x =>
                x.Id == assignmentId &&
                batchIds.Contains(x.BatchId));

        if (assignment == null)
       {
            return false;
        }

        _context.Assignments.Remove(assignment);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<MentorSubmissionDto>> GetSubmissionsAsync(
        Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        return await _context.AssignmentSubmissions
            .Include(x => x.Assignment)
            .Include(x => x.Student)
            .Where(x => batchIds.Contains(x.Assignment.BatchId))
            .OrderByDescending(x => x.SubmittedAt)
            .Select(x => new MentorSubmissionDto
           {
                SubmissionId = x.Id,
                AssignmentId = x.AssignmentId,
                BatchId = x.Assignment.BatchId,
                AssignmentTitle = x.Assignment.Title,
                StudentName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                SubmissionLink = x.SubmissionLink,
                Notes = x.Notes,
                Status = x.Status,
                SubmittedAt = x.SubmittedAt,
                Feedback = x.Feedback
            })
            .ToListAsync();
    }

    public async Task<MentorSubmissionDto?> ReviewSubmissionAsync(
        Guid mentorUserId,
        Guid submissionId,
        ReviewAssignmentSubmissionDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var submission = await _context.AssignmentSubmissions
            .Include(x => x.Assignment)
            .FirstOrDefaultAsync(x =>
                x.Id == submissionId &&
                batchIds.Contains(x.Assignment.BatchId));

        if (submission == null)
       {
            return null;
        }

        submission.Status = dto.Status;
        submission.Feedback = dto.Feedback.Trim();
        submission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetSubmissionsAsync(mentorUserId))
            .FirstOrDefault(x => x.SubmissionId == submission.Id);
    }

    public async Task<List<MentorSessionDto>> GetSessionsAsync(Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var sessions = await _context.Sessions
            .Where(x => batchIds.Contains(x.BatchId))
            .OrderByDescending(x => x.StartTime)
            .ToListAsync();

        var result = new List<MentorSessionDto>();

        foreach (var session in sessions)
       {
            result.Add(await MapSessionAsync(session));
        }

        return result;
    }

    public async Task<MentorSessionDto?> CreateSessionAsync(
        Guid mentorUserId,
        CreateMentorSessionDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        if (!batchIds.Contains(dto.BatchId))
       {
            return null;
        }

        var session = new Session
       {
            BatchId = dto.BatchId,
            Title = dto.Title.Trim(),
            MeetLink = dto.MeetLink.Trim(),
            StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc)
        };

        _context.Sessions.Add(session);

        await _context.SaveChangesAsync();

        return await MapSessionAsync(session);
    }

    public async Task<List<MentorSessionDto>> CreateWeeklySessionsAsync(
        Guid mentorUserId,
        CreateMentorWeeklySessionsDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        if (!batchIds.Contains(dto.BatchId) ||
            string.IsNullOrWhiteSpace(dto.MeetLink) ||
            dto.Days.Count == 0 ||
            dto.EndTime <= dto.StartTime)
       {
            return new List<MentorSessionDto>();
        }

        var weekStart = dto.WeekStartDate.Date;
        var created = new List<Session>();

        foreach (var day in dto.Days.Distinct().OrderBy(x => x))
       {
            var offset = ((int)day - (int)weekStart.DayOfWeek + 7) % 7;
            var date = weekStart.AddDays(offset);
            var startTime = DateTime.SpecifyKind(
                date.Add(dto.StartTime),
                DateTimeKind.Utc);
            var endTime = DateTime.SpecifyKind(
                date.Add(dto.EndTime),
                DateTimeKind.Utc);

            var session = new Session
           {
                BatchId = dto.BatchId,
                Title = $"{dto.TitlePrefix.Trim()} -{day}",
                MeetLink = dto.MeetLink.Trim(),
                StartTime = startTime,
                EndTime = endTime
            };

            _context.Sessions.Add(session);
            created.Add(session);
        }

        await _context.SaveChangesAsync();

        var result = new List<MentorSessionDto>();

        foreach (var session in created)
       {
            result.Add(await MapSessionAsync(session));
        }

        return result;
    }

    public async Task<bool> DeleteSessionAsync(
        Guid mentorUserId,
        Guid sessionId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var session = await _context.Sessions
            .FirstOrDefaultAsync(x =>
                x.Id == sessionId &&
                batchIds.Contains(x.BatchId));

        if (session == null)
       {
            return false;
        }

        var attendances = await _context.Attendances
            .Where(x => x.SessionId == sessionId)
            .ToListAsync();

        _context.Attendances.RemoveRange(attendances);
        _context.Sessions.Remove(session);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<MentorAttendanceRosterDto?> GetAttendanceRosterAsync(
        Guid mentorUserId,
        Guid sessionId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var session = await _context.Sessions
            .FirstOrDefaultAsync(x =>
                x.Id == sessionId &&
                batchIds.Contains(x.BatchId));

        if (session == null)
       {
            return null;
        }

        var students = await _context.Students
            .Where(x => x.BatchId == session.BatchId)
            .OrderBy(x => x.FullName)
            .ToListAsync();

        var attendance = await _context.Attendances
            .Where(x => x.SessionId == sessionId)
            .ToDictionaryAsync(x => x.StudentId, x => x.Status);

        return new MentorAttendanceRosterDto
       {
            SessionId = session.Id,
            SessionTitle = session.Title,
            Students = students.Select(x => new MentorAttendanceEntryDto
           {
                StudentId = x.Id,
                StudentName = x.FullName,
                Email = x.Email,
                Status = attendance.TryGetValue(x.Id, out var status)
                    ? status
                    : null
            }).ToList()
        };
    }

    public async Task<MentorAttendanceRosterDto?> MarkAttendanceAsync(
        Guid mentorUserId,
        Guid sessionId,
        MarkMentorAttendanceDto dto)
   {
        var roster = await GetAttendanceRosterAsync(mentorUserId, sessionId);

        if (roster == null)
       {
            return null;
        }

        var allowedStudentIds = roster.Students
            .Select(x => x.StudentId)
            .ToHashSet();

        foreach (var record in dto.Records)
       {
            if (!allowedStudentIds.Contains(record.StudentId))
           {
                continue;
            }

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(x =>
                    x.SessionId == sessionId &&
                    x.StudentId == record.StudentId);

            if (attendance == null)
           {
                attendance = new Attendance
               {
                    SessionId = sessionId,
                    StudentId = record.StudentId
                };

                _context.Attendances.Add(attendance);
            }

            attendance.Status = record.Status;
            attendance.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await GetAttendanceRosterAsync(mentorUserId, sessionId);
    }

    public async Task<List<MentorStudentReportDto>> GetStudentReportsAsync(
        Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var students = await _context.Students
            .Include(x => x.Batch)
            .Include(x => x.Track)
            .Where(x => batchIds.Contains(x.BatchId))
            .OrderBy(x => x.Batch.Name)
            .ThenBy(x => x.FullName)
            .ToListAsync();

        var result = new List<MentorStudentReportDto>();

        foreach (var student in students)
       {
            var attendanceRecords = await _context.Attendances
                .Where(x => x.StudentId == student.Id)
                .ToListAsync();

            var assignments = await _context.Assignments
                .Where(x =>
                    x.BatchId == student.BatchId &&
                    x.IsPublished)
                .ToListAsync();

            var assignmentIds = assignments.Select(x => x.Id).ToList();
            var submissions = await _context.AssignmentSubmissions
                .Where(x =>
                    x.StudentId == student.Id &&
                    assignmentIds.Contains(x.AssignmentId))
                .ToListAsync();

            var projectAssignments = assignments
                .Where(x =>
                    x.Title.Contains("project", StringComparison.OrdinalIgnoreCase) ||
                    x.Description.Contains("project", StringComparison.OrdinalIgnoreCase))
                .ToList();
            var projectIds = projectAssignments.Select(x => x.Id).ToList();
            var projectSubmissions = submissions
                .Where(x => projectIds.Contains(x.AssignmentId))
                .ToList();

            var studentModules = await _context.StudentModules
                .Where(x => x.StudentId == student.Id)
                .ToListAsync();

            var progress = studentModules.Count == 0
                ? 0
                : studentModules.Average(x => x.ProgressPercentage);

            result.Add(new MentorStudentReportDto
           {
                StudentId = student.Id,
                BatchId = student.BatchId,
                StudentName = student.FullName,
                Email = student.Email,
                BatchName = student.Batch.Name,
                TrackName = student.Track.Name,
                AttendancePercentage = await GetAttendancePercentageAsync(
                    new List<Guid>{ student.Id }),
                AttendanceMarkedCount = attendanceRecords.Count,
                PresentCount = attendanceRecords.Count(x =>
                    x.Status == AttendanceStatus.Present),
                LateCount = attendanceRecords.Count(x =>
                    x.Status == AttendanceStatus.Late),
                AbsentCount = attendanceRecords.Count(x =>
                    x.Status == AttendanceStatus.Absent),
                AssignmentCount = assignments.Count,
                SubmittedAssignmentCount = submissions.Count,
                ReviewedAssignmentCount = submissions.Count(x =>
                    x.Status == AssignmentSubmissionStatus.Reviewed),
                PendingAssignmentCount = assignments.Count - submissions.Count(x =>
                    x.Status == AssignmentSubmissionStatus.Reviewed),
                ProjectCount = projectAssignments.Count,
                SubmittedProjectCount = projectSubmissions.Count,
                ReviewedProjectCount = projectSubmissions.Count(x =>
                    x.Status == AssignmentSubmissionStatus.Reviewed),
                ExamPassedCount = studentModules.Count(x => x.ExamPassed),
                ExamPendingCount = studentModules.Count(x => !x.ExamPassed),
                ExamAttempts = studentModules.Sum(x => x.ExamAttempts),
                ProgressPercentage = Math.Round(progress, 2)
            });
        }

        return result;
    }

    public async Task<List<MentorSupportTicketDto>> GetSupportTicketsAsync(
        Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        return await _context.SupportTickets
            .Include(x => x.Student)
                .ThenInclude(x => x.Batch)
            .Where(x => batchIds.Contains(x.Student.BatchId))
            .OrderBy(x => x.Status == SupportTicketStatus.Resolved)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new MentorSupportTicketDto
           {
                Id = x.Id,
                StudentName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                StudentPhoneNumber = x.Student.PhoneNumber,
                BatchId = x.Student.BatchId,
                BatchName = x.Student.Batch.Name,
                Subject = x.Subject,
                Message = x.Message,
                Status = x.Status,
                MentorResponse = x.MentorResponse,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<MentorSupportTicketDto?> RespondToSupportTicketAsync(
        Guid mentorUserId,
        Guid ticketId,
        RespondSupportTicketDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var ticket = await _context.SupportTickets
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x =>
                x.Id == ticketId &&
                batchIds.Contains(x.Student.BatchId));

        if (ticket == null)
       {
            return null;
        }

        ticket.Status = dto.Status;
        ticket.MentorResponse = dto.MentorResponse.Trim();
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetSupportTicketsAsync(mentorUserId))
            .FirstOrDefault(x => x.Id == ticket.Id);
    }

    private async Task<List<Guid>> GetMentorBatchIdsAsync(Guid mentorUserId)
   {
        return await _context.Batches
            .Where(x =>
                x.IsActive &&
                (x.MentorId == mentorUserId ||
                 x.AssistantMentorId == mentorUserId))
            .Select(x => x.Id)
            .ToListAsync();
    }

    private async Task<List<Guid>> GetMentorLessonIdsAsync(Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        var trackIds = await _context.Batches
            .Where(x => batchIds.Contains(x.Id))
            .Select(x => x.TrackId)
            .Distinct()
            .ToListAsync();

        var moduleIds = await _context.TrackModules
            .Where(x => trackIds.Contains(x.TrackId))
            .Select(x => x.ModuleId)
            .Distinct()
            .ToListAsync();

        return await _context.Lessons
            .Where(x => moduleIds.Contains(x.ModuleId))
            .Select(x => x.Id)
            .ToListAsync();
    }

    private async Task<decimal> GetAttendancePercentageAsync(
        List<Guid> studentIds)
   {
        if (studentIds.Count == 0)
       {
            return 0;
        }

        var records = await _context.Attendances
            .Where(x => studentIds.Contains(x.StudentId))
            .ToListAsync();

        if (records.Count == 0)
       {
            return 0;
        }

        var attended = records.Count(x =>
            x.Status == AttendanceStatus.Present ||
            x.Status == AttendanceStatus.Late);

        return Math.Round((attended * 100m) / records.Count, 2);
    }

    private async Task<MentorSessionDto> MapSessionAsync(Session session)
   {
        var batch = await _context.Batches
            .FirstAsync(x => x.Id == session.BatchId);

        var studentCount = await _context.Students
            .CountAsync(x => x.BatchId == session.BatchId);

        var attendance = await _context.Attendances
            .Where(x => x.SessionId == session.Id)
            .ToListAsync();

        return new MentorSessionDto
       {
            SessionId = session.Id,
            BatchId = session.BatchId,
            BatchName = batch.Name,
            Title = session.Title,
            MeetLink = session.MeetLink,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            StudentCount = studentCount,
            MarkedCount = attendance.Count,
            PresentCount = attendance.Count(x =>
                x.Status == AttendanceStatus.Present),
            LateCount = attendance.Count(x =>
                x.Status == AttendanceStatus.Late),
            AbsentCount = attendance.Count(x =>
                x.Status == AttendanceStatus.Absent)
        };
    }
}
