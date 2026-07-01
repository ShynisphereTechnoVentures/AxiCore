using AxiPlus.Domain.Enums;

namespace AxiPlus.Application.DTOs.AdminPortal;

public class AdminDashboardDto
{
    public int UserCount{ get; set; }

    public int ActiveUserCount{ get; set; }

    public int StudentCount{ get; set; }

    public int MentorCount{ get; set; }

    public int BatchCount{ get; set; }

    public int TrackCount{ get; set; }

    public int ModuleCount{ get; set; }

    public int OpenSupportTicketCount{ get; set; }

    public int PendingSubmissionCount{ get; set; }

    public int OpenAttendanceIssueCount{ get; set; }

    public int PaymentDueCount{ get; set; }

    public int LockedStudentCount{ get; set; }

    public decimal AttendancePercentage{ get; set; }
}

public class AdminUserDto
{
    public Guid Id{ get; set; }

    public string FullName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public int RoleId{ get; set; }

    public string Role{ get; set; } = string.Empty;

    public bool IsActive{ get; set; }
}

public class CreateAdminUserDto
{
    public string FullName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public string Password{ get; set; } = string.Empty;

    public int RoleId{ get; set; }
}

public class UpdateUserStatusDto
{
    public bool IsActive{ get; set; }
}

public class AdminRoleDto
{
    public int Id{ get; set; }

    public string Name{ get; set; } = string.Empty;
}

public class AdminTrackDto
{
    public int Id{ get; set; }

    public string Name{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public string BatchPrefix{ get; set; } = string.Empty;

    public bool IsActive{ get; set; }

    public int ModuleCount{ get; set; }
}

public class CreateAdminTrackDto
{
    public string Name{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public string BatchPrefix{ get; set; } = string.Empty;
}

public class AdminBatchDto
{
    public Guid Id{ get; set; }

    public string Name{ get; set; } = string.Empty;

    public int BatchNumber{ get; set; }

    public int TrackId{ get; set; }

    public string TrackName{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public int Capacity{ get; set; }

    public int CurrentStrength{ get; set; }

    public bool IsActive{ get; set; }

    public Guid? MentorId{ get; set; }

    public string MentorName{ get; set; } = string.Empty;

    public Guid? AssistantMentorId{ get; set; }

    public string AssistantMentorName{ get; set; } = string.Empty;
}

public class CreateAdminBatchDto
{
    public string Name{ get; set; } = string.Empty;

    public int BatchNumber{ get; set; }

    public int TrackId{ get; set; }

    public string Level{ get; set; } = string.Empty;

    public int Capacity{ get; set; } = 15;

    public Guid? MentorId{ get; set; }

    public Guid? AssistantMentorId{ get; set; }
}

public class UpdateAdminBatchMentorsDto
{
    public Guid? MentorId{ get; set; }

    public Guid? AssistantMentorId{ get; set; }
}

public class AdminStudentDto
{
    public Guid Id{ get; set; }

    public Guid UserId{ get; set; }

    public Guid BatchId{ get; set; }

    public string FullName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public string PhoneNumber{ get; set; } = string.Empty;

    public string CollegeName{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public string TrackName{ get; set; } = string.Empty;

    public DateTime JoinedDate{ get; set; }

    public decimal ProgressPercentage{ get; set; }

    public decimal AttendancePercentage{ get; set; }

    public BillingStatus BillingStatus{ get; set; }

    public decimal MonthlyFee{ get; set; }

    public DateTime? NextDueDate{ get; set; }

    public DateTime? GraceEndsAt{ get; set; }

    public bool AutoPayEnabled{ get; set; }
}

public class UpdateAdminStudentBillingStatusDto
{
    public BillingStatus Status{ get; set; }
}

public class AdminModuleDto
{
    public int Id{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public int Order{ get; set; }

    public bool IsActive{ get; set; }

    public bool IsPublished{ get; set; }

    public int LessonCount{ get; set; }
}

public class CreateAdminModuleDto
{
    public string Title{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public int Order{ get; set; }

    public bool IsPublished{ get; set; } = true;
}

public class AdminSupportTicketDto
{
    public Guid Id{ get; set; }

    public string StudentName{ get; set; } = string.Empty;

    public string StudentEmail{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public string Subject{ get; set; } = string.Empty;

    public string Message{ get; set; } = string.Empty;

    public SupportTicketStatus Status{ get; set; }

    public string MentorResponse{ get; set; } = string.Empty;

    public DateTime CreatedAt{ get; set; }
}

public class AdminAssignmentSubmissionDto
{
    public Guid Id{ get; set; }

    public string AssignmentTitle{ get; set; } = string.Empty;

    public string StudentName{ get; set; } = string.Empty;

    public string StudentEmail{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public string SubmissionLink{ get; set; } = string.Empty;

    public string Notes{ get; set; } = string.Empty;

    public AssignmentSubmissionStatus Status{ get; set; }

    public DateTime SubmittedAt{ get; set; }

    public string Feedback{ get; set; } = string.Empty;
}

public class AdminAttendanceDiscrepancyDto
{
    public Guid Id{ get; set; }

    public string StudentName{ get; set; } = string.Empty;

    public string StudentEmail{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public string SessionTitle{ get; set; } = string.Empty;

    public AttendanceStatus? CurrentStatus{ get; set; }

    public AttendanceStatus RequestedStatus{ get; set; }

    public string Reason{ get; set; } = string.Empty;

    public AttendanceDiscrepancyStatus Status{ get; set; }

    public string ResolutionNote{ get; set; } = string.Empty;

    public DateTime CreatedAt{ get; set; }
}

public class AdminPaymentDto
{
    public Guid Id{ get; set; }

    public string StudentName{ get; set; } = string.Empty;

    public string StudentEmail{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public decimal Amount{ get; set; }

    public string Currency{ get; set; } = "INR";

    public PaymentStatus Status{ get; set; }

    public string Method{ get; set; } = string.Empty;

    public string ProviderReference{ get; set; } = string.Empty;

    public DateTime DueDate{ get; set; }

    public DateTime GraceEndsAt{ get; set; }

    public DateTime? PaidAt{ get; set; }
}

public class AdminBatchBillingDto
{
    public Guid BatchId{ get; set; }

    public string BatchName{ get; set; } = string.Empty;

    public string TrackName{ get; set; } = string.Empty;

    public int TotalStudents{ get; set; }

    public int PaidStudentCount{ get; set; }

    public int UnpaidStudentCount{ get; set; }

    public decimal PendingAmount{ get; set; }

    public List<AdminBatchBillingStudentDto> UnpaidStudents{ get; set; }
        = new();
}

public class AdminBatchBillingStudentDto
{
    public Guid StudentId{ get; set; }

    public string FullName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public string PhoneNumber{ get; set; } = string.Empty;

    public BillingStatus BillingStatus{ get; set; }

    public decimal MonthlyFee{ get; set; }

    public DateTime? NextDueDate{ get; set; }

    public DateTime? GraceEndsAt{ get; set; }

    public bool AutoPayEnabled{ get; set; }
}
