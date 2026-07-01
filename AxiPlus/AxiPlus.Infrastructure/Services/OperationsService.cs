using AxiPlus.Application.DTOs.Operations;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class OperationsService : IOperationsService
{        
    private readonly AppDbContext _context;

    public OperationsService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<MentorProfileDto?> GetMentorProfileAsync(Guid userId)
   {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
       {
            return null;
        }

        var profile = await _context.MentorProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        return new MentorProfileDto
       {        
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name,
            PhoneNumber = profile?.PhoneNumber ?? string.Empty,
            Address = profile?.Address ?? string.Empty,
            EmergencyContact = profile?.EmergencyContact ?? string.Empty,
            Designation = profile?.Designation ?? user.Role.Name,
            JoinedDate = profile?.JoinedDate ?? user.CreatedAt,
            SalarySlips = await GetSalarySlipsAsync(userId)
        };
    }

    public async Task<List<SalarySlipDto>> GetSalarySlipsAsync(Guid userId)
   {
        return await _context.SalarySlips
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .Select(x => MapSalarySlip(x))
            .ToListAsync();
    }

    public async Task<SalarySlipDto?> CreateSalarySlipAsync(
        Guid adminUserId,
        CreateSalarySlipDto dto)
   {
        var userExists = await _context.Users.AnyAsync(x => x.Id == dto.UserId);

        if (!userExists ||
            dto.Month < 1 ||
            dto.Month > 12 ||
            dto.Year < 2000 ||
            string.IsNullOrWhiteSpace(dto.FileUrl))
       {
            return null;
        }

        var slip = new SalarySlip
       {        
            UserId = dto.UserId,
            Month = dto.Month,
            Year = dto.Year,
            GrossAmount = dto.GrossAmount,
            NetAmount = dto.NetAmount,
            FileUrl = dto.FileUrl.Trim(),
            UploadedByUserId = adminUserId
        };

        _context.SalarySlips.Add(slip);
        await _context.SaveChangesAsync();

        return await _context.SalarySlips
            .Include(x => x.User)
            .Where(x => x.Id == slip.Id)
            .Select(x => MapSalarySlip(x))
            .FirstOrDefaultAsync();
    }

    public async Task<List<MeetingRequestDto>> GetMentorMeetingRequestsAsync(
        Guid mentorUserId)
   {
        return await _context.MeetingRequests
            .Include(x => x.Student)
            .Include(x => x.MentorUser)
            .Include(x => x.Batch)
            .Where(x => x.MentorUserId == mentorUserId)
            .OrderByDescending(x => x.ScheduledAt)
            .Select(x => MapMeetingRequest(x))
            .ToListAsync();
    }

    public async Task<List<MeetingRequestDto>> GetStudentMeetingRequestsAsync(
        Guid studentUserId)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.UserId == studentUserId);

        if (student == null)
       {
            return new List<MeetingRequestDto>();
        }

        return await _context.MeetingRequests
            .Include(x => x.Student)
            .Include(x => x.MentorUser)
            .Include(x => x.Batch)
            .Where(x => x.StudentId == student.Id)
            .OrderByDescending(x => x.ScheduledAt)
            .Select(x => MapMeetingRequest(x))
            .ToListAsync();
    }

    public async Task<MeetingRequestDto?> CreateMeetingRequestAsync(
        Guid mentorUserId,
        CreateMeetingRequestDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);
        var student = await _context.Students
            .FirstOrDefaultAsync(x =>
                x.Id == dto.StudentId &&
                batchIds.Contains(x.BatchId));

        if (student == null || string.IsNullOrWhiteSpace(dto.Reason))
       {
            return null;
        }

        var request = new MeetingRequest
       {        
            MentorUserId = mentorUserId,
            StudentId = student.Id,
            BatchId = student.BatchId,
            ScheduledAt = DateTime.SpecifyKind(dto.ScheduledAt, DateTimeKind.Utc),
            MeetingLink = dto.MeetingLink.Trim(),
            Reason = dto.Reason.Trim(),
            Status = MeetingRequestStatus.Pending
        };

        _context.MeetingRequests.Add(request);
        _context.StudentNotifications.Add(new StudentNotification
       {        
            StudentId = student.Id,
            Title = "Meeting request",
            Message = dto.Reason.Trim(),
            Type = "MeetingRequest"
        });

        await _context.SaveChangesAsync();

        return await GetMeetingRequestAsync(request.Id);
    }

    public async Task<MeetingRequestDto?> RespondToMeetingRequestAsync(
        Guid studentUserId,
        Guid meetingRequestId,
        RespondMeetingRequestDto dto)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.UserId == studentUserId);

        if (student == null)
       {
            return null;
        }

        var request = await _context.MeetingRequests
            .FirstOrDefaultAsync(x =>
                x.Id == meetingRequestId &&
                x.StudentId == student.Id);

        if (request == null)
       {
            return null;
        }

        request.Status = dto.Status;
        request.StudentResponseNote = dto.StudentResponseNote.Trim();
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetMeetingRequestAsync(request.Id);
    }

    public async Task<MeetingRequestDto?> UpdateMeetingFollowUpAsync(
        Guid mentorUserId,
        Guid meetingRequestId,
        UpdateMeetingFollowUpDto dto)
   {
        var request = await _context.MeetingRequests
            .FirstOrDefaultAsync(x =>
                x.Id == meetingRequestId &&
                x.MentorUserId == mentorUserId);

        if (request == null)
       {
            return null;
        }

        request.Status = dto.Status;
        request.MentorFollowUpNote = dto.MentorFollowUpNote.Trim();
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetMeetingRequestAsync(request.Id);
    }

    public async Task<List<AttendanceDiscrepancyDto>>
        GetStudentAttendanceDiscrepanciesAsync(Guid studentUserId)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.UserId == studentUserId);

        if (student == null)
       {
            return new List<AttendanceDiscrepancyDto>();
        }

        return await _context.AttendanceDiscrepancies
            .Include(x => x.Student)
            .Include(x => x.Session)
            .Where(x => x.StudentId == student.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => MapAttendanceDiscrepancy(x))
            .ToListAsync();
    }

    public async Task<List<AttendanceDiscrepancyDto>>
        GetMentorAttendanceDiscrepanciesAsync(Guid mentorUserId)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);

        return await _context.AttendanceDiscrepancies
            .Include(x => x.Student)
            .Include(x => x.Session)
            .Where(x => batchIds.Contains(x.Student.BatchId))
            .OrderBy(x => x.Status == AttendanceDiscrepancyStatus.Approved ||
                x.Status == AttendanceDiscrepancyStatus.Rejected)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => MapAttendanceDiscrepancy(x))
            .ToListAsync();
    }

    public async Task<AttendanceDiscrepancyDto?> CreateAttendanceDiscrepancyAsync(
        Guid studentUserId,
        CreateAttendanceDiscrepancyDto dto)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.UserId == studentUserId);

        if (student == null)
       {
            return null;
        }

        var session = await _context.Sessions
            .FirstOrDefaultAsync(x =>
                x.Id == dto.SessionId &&
                x.BatchId == student.BatchId);

        if (session == null || string.IsNullOrWhiteSpace(dto.Reason))
       {
            return null;
        }

        var attendance = await _context.Attendances
            .FirstOrDefaultAsync(x =>
                x.SessionId == session.Id &&
                x.StudentId == student.Id);

        var discrepancy = new AttendanceDiscrepancy
       {        
            StudentId = student.Id,
            SessionId = session.Id,
            CurrentStatus = attendance?.Status,
            RequestedStatus = dto.RequestedStatus,
            Reason = dto.Reason.Trim(),
            Status = AttendanceDiscrepancyStatus.Open
        };

        _context.AttendanceDiscrepancies.Add(discrepancy);
        await _context.SaveChangesAsync();

        return await GetAttendanceDiscrepancyAsync(discrepancy.Id);
    }

    public async Task<AttendanceDiscrepancyDto?> ReviewAttendanceDiscrepancyAsync(
        Guid mentorUserId,
        Guid discrepancyId,
        ReviewAttendanceDiscrepancyDto dto)
   {
        var batchIds = await GetMentorBatchIdsAsync(mentorUserId);
        var discrepancy = await _context.AttendanceDiscrepancies
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x =>
                x.Id == discrepancyId &&
                batchIds.Contains(x.Student.BatchId));

        if (discrepancy == null)
       {
            return null;
        }

        discrepancy.Status = dto.Status;
        discrepancy.ResolutionNote = dto.ResolutionNote.Trim();
        discrepancy.ReviewedByUserId = mentorUserId;
        discrepancy.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == AttendanceDiscrepancyStatus.Approved)
       {
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(x =>
                    x.SessionId == discrepancy.SessionId &&
                    x.StudentId == discrepancy.StudentId);

            if (attendance == null)
           {
                attendance = new Attendance
               {        
                    SessionId = discrepancy.SessionId,
                    StudentId = discrepancy.StudentId
                };

                _context.Attendances.Add(attendance);
            }

            attendance.Status = discrepancy.RequestedStatus;
            attendance.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await GetAttendanceDiscrepancyAsync(discrepancy.Id);
    }

    private async Task<MeetingRequestDto?> GetMeetingRequestAsync(Guid id)
   {
        return await _context.MeetingRequests
            .Include(x => x.Student)
            .Include(x => x.MentorUser)
            .Include(x => x.Batch)
            .Where(x => x.Id == id)
            .Select(x => MapMeetingRequest(x))
            .FirstOrDefaultAsync();
    }

    private async Task<AttendanceDiscrepancyDto?> GetAttendanceDiscrepancyAsync(
        Guid id)
   {
        return await _context.AttendanceDiscrepancies
            .Include(x => x.Student)
            .Include(x => x.Session)
            .Where(x => x.Id == id)
            .Select(x => MapAttendanceDiscrepancy(x))
            .FirstOrDefaultAsync();
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

    private static SalarySlipDto MapSalarySlip(SalarySlip slip)
   {
        return new SalarySlipDto
       {        
            Id = slip.Id,
            UserId = slip.UserId,
            UserName = slip.User.FullName,
            Month = slip.Month,
            Year = slip.Year,
            GrossAmount = slip.GrossAmount,
            NetAmount = slip.NetAmount,
            FileUrl = slip.FileUrl
        };
    }

    private static MeetingRequestDto MapMeetingRequest(MeetingRequest request)
   {
        return new MeetingRequestDto
       {        
            Id = request.Id,
            StudentId = request.StudentId,
            StudentName = request.Student.FullName,
            StudentPhoneNumber = request.Student.PhoneNumber,
            MentorUserId = request.MentorUserId,
            MentorName = request.MentorUser.FullName,
            BatchName = request.Batch.Name,
            ScheduledAt = request.ScheduledAt,
            MeetingLink = request.MeetingLink,
            Reason = request.Reason,
            Status = request.Status,
            StudentResponseNote = request.StudentResponseNote,
            MentorFollowUpNote = request.MentorFollowUpNote
        };
    }

    private static AttendanceDiscrepancyDto MapAttendanceDiscrepancy(
        AttendanceDiscrepancy discrepancy)
   {
        return new AttendanceDiscrepancyDto
       {        
            Id = discrepancy.Id,
            StudentId = discrepancy.StudentId,
            StudentName = discrepancy.Student.FullName,
            SessionId = discrepancy.SessionId,
            SessionTitle = discrepancy.Session.Title,
            CurrentStatus = discrepancy.CurrentStatus,
            RequestedStatus = discrepancy.RequestedStatus,
            Reason = discrepancy.Reason,
            Status = discrepancy.Status,
            ResolutionNote = discrepancy.ResolutionNote
        };
    }
}
