using AxiPlus.Application.DTOs.AdminPortal;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class AdminPortalService : IAdminPortalService
{       
    private readonly AppDbContext _context;

    public AdminPortalService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
   {
        var attendanceRecords = await _context.Attendances
            .ToListAsync();

        var attended = attendanceRecords.Count(x =>
            x.Status == AttendanceStatus.Present ||
            x.Status == AttendanceStatus.Late);

        var attendancePercentage = attendanceRecords.Count == 0
            ? 0
            : Math.Round(attended * 100m / attendanceRecords.Count, 2);

        return new AdminDashboardDto
       {       
            UserCount = await _context.Users.CountAsync(),
            ActiveUserCount = await _context.Users.CountAsync(x => x.IsActive),
            StudentCount = await _context.Students.CountAsync(),
            MentorCount = await _context.Users.CountAsync(x =>
                x.Role.Name == "MainMentor" ||
                x.Role.Name == "AssistantMentor"),
            BatchCount = await _context.Batches.CountAsync(),
            TrackCount = await _context.Tracks.CountAsync(),
            ModuleCount = await _context.Modules.CountAsync(),
            OpenSupportTicketCount = await _context.SupportTickets.CountAsync(x =>
                x.Status != SupportTicketStatus.Resolved),
            PendingSubmissionCount = await _context.AssignmentSubmissions
                .CountAsync(x => x.Status == AssignmentSubmissionStatus.Submitted),
            OpenAttendanceIssueCount = await _context.AttendanceDiscrepancies
                .CountAsync(x =>
                    x.Status == AttendanceDiscrepancyStatus.Open ||
                    x.Status == AttendanceDiscrepancyStatus.InReview),
            PaymentDueCount = await _context.StudentBillingAccounts
                .CountAsync(x =>
                    x.Status == BillingStatus.PaymentDue ||
                    x.Status == BillingStatus.GracePeriod),
            LockedStudentCount = await _context.StudentBillingAccounts
                .CountAsync(x => x.Status == BillingStatus.Locked),
            AttendancePercentage = attendancePercentage
        };
    }

    public async Task<List<AdminRoleDto>> GetRolesAsync()
   {
        return await _context.Roles
            .OrderBy(x => x.Id)
            .Select(x => new AdminRoleDto
           {      
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<List<AdminUserDto>> GetUsersAsync()
   {
        return await _context.Users
            .Include(x => x.Role)
            .OrderBy(x => x.RoleId)
            .ThenBy(x => x.FullName)
            .Select(x => MapUser(x))
            .ToListAsync();
    }

    public async Task<AdminUserDto?> CreateUserAsync(CreateAdminUserDto dto)
   {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(x => x.Email.ToLower() == email))
       {
            return null;
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == dto.RoleId);

        if (role == null)
       {
            return null;
        }

        var user = new User
       {       
            Id = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            Email = email,
            RoleId = role.Id,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        user.Role = role;

        return MapUser(user);
    }

    public async Task<AdminUserDto?> UpdateUserStatusAsync(
        Guid userId,
        UpdateUserStatusDto dto)
   {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
       {
            return null;
        }

        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapUser(user);
    }

    public async Task<List<AdminTrackDto>> GetTracksAsync()
   {
        return await _context.Tracks
            .OrderBy(x => x.Name)
            .Select(x => new AdminTrackDto
           {     
                Id = x.Id,
                Name = x.Name,
                Level = x.Level,
                BatchPrefix = x.BatchPrefix,
                IsActive = x.IsActive,
                ModuleCount = x.TrackModules.Count
            })
            .ToListAsync();
    }

    public async Task<AdminTrackDto?> CreateTrackAsync(CreateAdminTrackDto dto)
   {
        var name = dto.Name.Trim();

        if (await _context.Tracks.AnyAsync(x => x.Name == name))
       {
            return null;
        }

        var track = new Track
       {     
            Name = name,
            Level = dto.Level.Trim(),
            BatchPrefix = dto.BatchPrefix.Trim().ToUpperInvariant(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tracks.Add(track);

        await _context.SaveChangesAsync();

        return (await GetTracksAsync()).FirstOrDefault(x => x.Id == track.Id);
    }

    public async Task<List<AdminBatchDto>> GetBatchesAsync()
   {
        return await _context.Batches
            .Include(x => x.Track)
            .Include(x => x.Mentor)
            .Include(x => x.AssistantMentor)
            .OrderBy(x => x.Name)
            .Select(x => MapBatch(x))
            .ToListAsync();
    }

    public async Task<AdminBatchDto?> CreateBatchAsync(CreateAdminBatchDto dto)
   {
        var track = await _context.Tracks
            .FirstOrDefaultAsync(x => x.Id == dto.TrackId);

        if (track == null)
       {
            return null;
        }

        if (dto.MentorId.HasValue &&
            !await UserHasRoleAsync(dto.MentorId.Value, "MainMentor"))
       {
            return null;
        }

        if (dto.AssistantMentorId.HasValue &&
            !await UserHasRoleAsync(dto.AssistantMentorId.Value, "AssistantMentor"))
       {
            return null;
        }

        var batch = new Batch
       {       
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            BatchNumber = dto.BatchNumber,
            TrackId = track.Id,
            Level = string.IsNullOrWhiteSpace(dto.Level)
                ? track.Level
                : dto.Level.Trim(),
            Capacity = Math.Max(dto.Capacity, 1),
            CurrentStrength = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            MentorId = dto.MentorId,
            AssistantMentorId = dto.AssistantMentorId
        };

        _context.Batches.Add(batch);

        await _context.SaveChangesAsync();

        return (await GetBatchesAsync()).FirstOrDefault(x => x.Id == batch.Id);
    }

    public async Task<AdminBatchDto?> UpdateBatchMentorsAsync(
        Guid batchId,
        UpdateAdminBatchMentorsDto dto)
   {
        var batch = await _context.Batches
            .FirstOrDefaultAsync(x => x.Id == batchId);

        if (batch == null)
       {
            return null;
        }

        if (dto.MentorId.HasValue &&
            !await UserHasRoleAsync(dto.MentorId.Value, "MainMentor"))
       {
            return null;
        }

        if (dto.AssistantMentorId.HasValue &&
            !await UserHasRoleAsync(dto.AssistantMentorId.Value, "AssistantMentor"))
       {
            return null;
        }

        batch.MentorId = dto.MentorId;
        batch.AssistantMentorId = dto.AssistantMentorId;

        await _context.SaveChangesAsync();

        return (await GetBatchesAsync()).FirstOrDefault(x => x.Id == batch.Id);
    }

    public async Task<List<AdminStudentDto>> GetStudentsAsync()
   {
        var students = await _context.Students
            .Include(x => x.Batch)
            .Include(x => x.Track)
            .Include(x => x.BillingAccount)
            .OrderBy(x => x.Batch.Name)
            .ThenBy(x => x.FullName)
            .ToListAsync();

        var result = new List<AdminStudentDto>();

        foreach (var student in students)
       {
            var progress = await _context.StudentModules
                .Where(x => x.StudentId == student.Id)
                .AverageAsync(x => (decimal?)x.ProgressPercentage) ?? 0;

            result.Add(new AdminStudentDto
           {       
                Id = student.Id,
                UserId = student.UserId,
                BatchId = student.BatchId,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                CollegeName = student.CollegeName,
                BatchName = student.Batch.Name,
                TrackName = student.Track.Name,
                JoinedDate = student.JoinedDate,
                ProgressPercentage = Math.Round(progress, 2),
                AttendancePercentage =
                    await GetAttendancePercentageAsync(student.Id),
                BillingStatus = student.BillingAccount?.Status
                    ?? BillingStatus.Trial,
                MonthlyFee = student.BillingAccount?.MonthlyFee ?? 0,
                NextDueDate = student.BillingAccount?.NextDueDate,
                GraceEndsAt = student.BillingAccount?.GraceEndsAt,
                AutoPayEnabled = student.BillingAccount?.AutoPayEnabled ?? false
            });
        }

        return result;
    }

    public async Task<AdminStudentDto?> UpdateStudentBillingStatusAsync(
        Guid studentId,
        UpdateAdminStudentBillingStatusDto dto)
   {
        var student = await _context.Students
            .Include(x => x.BillingAccount)
            .FirstOrDefaultAsync(x => x.Id == studentId);

        if (student == null)
       {
            return null;
        }

        if (student.BillingAccount == null)
       {
            student.BillingAccount = new StudentBillingAccount
           {       
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                MonthlyFee = 0,
                Currency = "INR",
                NextDueDate = DateTime.UtcNow.AddDays(15),
                GraceEndsAt = DateTime.UtcNow.AddDays(30),
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.StudentBillingAccounts.Add(student.BillingAccount);
        }
        else
       {
            student.BillingAccount.Status = dto.Status;
            student.BillingAccount.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return (await GetStudentsAsync()).FirstOrDefault(x => x.Id == studentId);
    }

    public async Task<List<AdminModuleDto>> GetModulesAsync()
   {
        return await _context.Modules
            .OrderBy(x => x.Order)
            .Select(x => new AdminModuleDto
           {     
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Order = x.Order,
                IsActive = x.IsActive,
                IsPublished = x.IsPublished,
                LessonCount = x.Lessons.Count
            })
            .ToListAsync();
    }

    public async Task<AdminModuleDto?> CreateModuleAsync(
        CreateAdminModuleDto dto)
   {
        var module = new Module
       {       
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Order = dto.Order,
            IsActive = true,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        _context.Modules.Add(module);

        await _context.SaveChangesAsync();

        return (await GetModulesAsync()).FirstOrDefault(x => x.Id == module.Id);
    }

    public async Task<List<AdminSupportTicketDto>> GetSupportTicketsAsync()
   {
        return await _context.SupportTickets
            .Include(x => x.Student)
                .ThenInclude(x => x.Batch)
            .OrderBy(x => x.Status == SupportTicketStatus.Resolved)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new AdminSupportTicketDto
           {     
                Id = x.Id,
                StudentName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                BatchName = x.Student.Batch.Name,
                Subject = x.Subject,
                Message = x.Message,
                Status = x.Status,
                MentorResponse = x.MentorResponse,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<AdminAssignmentSubmissionDto>>
        GetAssignmentSubmissionsAsync()
   {
        return await _context.AssignmentSubmissions
            .Include(x => x.Assignment)
                .ThenInclude(x => x.Batch)
            .Include(x => x.Student)
            .OrderBy(x => x.Status == AssignmentSubmissionStatus.Reviewed)
            .ThenByDescending(x => x.SubmittedAt)
            .Select(x => new AdminAssignmentSubmissionDto
           {        
                Id = x.Id,
                AssignmentTitle = x.Assignment.Title,
                StudentName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                BatchName = x.Assignment.Batch.Name,
                SubmissionLink = x.SubmissionLink,
                Notes = x.Notes,
                Status = x.Status,
                SubmittedAt = x.SubmittedAt,
                Feedback = x.Feedback
            })
            .ToListAsync();
    }

    public async Task<List<AdminAttendanceDiscrepancyDto>>
        GetAttendanceDiscrepanciesAsync()
   {
        return await _context.AttendanceDiscrepancies
            .Include(x => x.Student)
                .ThenInclude(x => x.Batch)
            .Include(x => x.Session)
            .OrderBy(x =>
                x.Status == AttendanceDiscrepancyStatus.Approved ||
                x.Status == AttendanceDiscrepancyStatus.Rejected)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new AdminAttendanceDiscrepancyDto
           {      
                Id = x.Id,
                StudentName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                BatchName = x.Student.Batch.Name,
                SessionTitle = x.Session.Title,
                CurrentStatus = x.CurrentStatus,
                RequestedStatus = x.RequestedStatus,
                Reason = x.Reason,
                Status = x.Status,
                ResolutionNote = x.ResolutionNote,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<AdminPaymentDto>> GetPaymentsAsync()
   {
        return await _context.StudentPayments
            .Include(x => x.Student)
                .ThenInclude(x => x.Batch)
            .OrderBy(x => x.Status == PaymentStatus.Paid)
            .ThenBy(x => x.DueDate)
            .Select(x => new AdminPaymentDto
           {        
                Id = x.Id,
                StudentName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                BatchName = x.Student.Batch.Name,
                Amount = x.Amount,
                Currency = x.Currency,
                Status = x.Status,
                Method = x.Method,
                ProviderReference = x.ProviderReference,
                DueDate = x.DueDate,
                GraceEndsAt = x.GraceEndsAt,
                PaidAt = x.PaidAt
            })
            .ToListAsync();
    }

    public async Task<List<AdminBatchBillingDto>> GetBatchBillingAsync()
   {
        var batches = await _context.Batches
            .Include(x => x.Track)
            .OrderBy(x => x.Name)
            .ToListAsync();

        var students = await _context.Students
            .Include(x => x.BillingAccount)
            .ToListAsync();

        var result = new List<AdminBatchBillingDto>();

        foreach (var batch in batches)
       {
            var batchStudents = students
                .Where(x => x.BatchId == batch.Id)
                .OrderBy(x => x.FullName)
                .ToList();

            var unpaidStudents = batchStudents
                .Where(x => IsUnpaidBillingStatus(x.BillingAccount?.Status))
                .Select(x => new AdminBatchBillingStudentDto
               {      
                    StudentId = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    BillingStatus = x.BillingAccount?.Status
                        ?? BillingStatus.PaymentDue,
                    MonthlyFee = x.BillingAccount?.MonthlyFee ?? 0,
                    NextDueDate = x.BillingAccount?.NextDueDate,
                    GraceEndsAt = x.BillingAccount?.GraceEndsAt,
                    AutoPayEnabled = x.BillingAccount?.AutoPayEnabled ?? false
                })
                .ToList();

            result.Add(new AdminBatchBillingDto
           {     
                BatchId = batch.Id,
                BatchName = batch.Name,
                TrackName = batch.Track.Name,
                TotalStudents = batchStudents.Count,
                PaidStudentCount = batchStudents.Count - unpaidStudents.Count,
                UnpaidStudentCount = unpaidStudents.Count,
                PendingAmount = unpaidStudents.Sum(x => x.MonthlyFee),
                UnpaidStudents = unpaidStudents
            });
        }

        return result;
    }

    private static AdminUserDto MapUser(User user)
   {
        return new AdminUserDto
       {    
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,
            Role = user.Role.Name,
            IsActive = user.IsActive
        };
    }

    private static AdminBatchDto MapBatch(Batch batch)
   {
        return new AdminBatchDto
       {      
            Id = batch.Id,
            Name = batch.Name,
            BatchNumber = batch.BatchNumber,
            TrackId = batch.TrackId,
            TrackName = batch.Track.Name,
            Level = batch.Level,
            Capacity = batch.Capacity,
            CurrentStrength = batch.CurrentStrength,
            IsActive = batch.IsActive,
            MentorId = batch.MentorId,
            MentorName = batch.Mentor?.FullName ?? "Not Assigned",
            AssistantMentorId = batch.AssistantMentorId,
            AssistantMentorName =
                batch.AssistantMentor?.FullName ?? "Not Assigned"
        };
    }

    private async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
   {
        return await _context.Users
            .AnyAsync(x =>
                x.Id == userId &&
                x.IsActive &&
                x.Role.Name == roleName);
    }

    private async Task<decimal> GetAttendancePercentageAsync(Guid studentId)
   {
        var records = await _context.Attendances
            .Where(x => x.StudentId == studentId)
            .ToListAsync();

        if (records.Count == 0)
       {
            return 0;
        }

        var attended = records.Count(x =>
            x.Status == AttendanceStatus.Present ||
            x.Status == AttendanceStatus.Late);

        return Math.Round(attended * 100m / records.Count, 2);
    }

    private static bool IsUnpaidBillingStatus(BillingStatus? status)
   {
        return status is null ||
            status == BillingStatus.PaymentDue ||
            status == BillingStatus.GracePeriod ||
            status == BillingStatus.Locked;
    }
}
