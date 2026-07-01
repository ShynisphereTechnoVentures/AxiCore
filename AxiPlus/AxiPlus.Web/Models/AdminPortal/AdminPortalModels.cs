using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.AdminPortal;

public class AdminDashboardModel
{
    public int UserCount{get;set; }

    public int ActiveUserCount{get;set; }

    public int StudentCount{get;set; }

    public int MentorCount{get;set; }

    public int BatchCount{get;set; }

    public int TrackCount{get;set; }

    public int ModuleCount{get;set; }

    public int OpenSupportTicketCount{get;set; }

    public int PendingSubmissionCount{get;set; }

    public int OpenAttendanceIssueCount{get;set; }

    public int PaymentDueCount{get;set; }

    public int LockedStudentCount{get;set; }

    public decimal AttendancePercentage{get;set; }
}

public class AdminUserModel
{
    public Guid Id{get;set; }

    public string FullName{get;set; } = string.Empty;

    public string Email{get;set; } = string.Empty;

    public int RoleId{get;set; }

    public string Role{get;set; } = string.Empty;

    public bool IsActive{get;set; }
}

public class CreateAdminUserModel
{
    public string FullName{get;set; } = string.Empty;

    public string Email{get;set; } = string.Empty;

    public string Password{get;set; } = string.Empty;

    public int RoleId{get;set; }
}

public class UpdateUserStatusModel
{
    public bool IsActive{get;set; }
}

public class AdminRoleModel
{
    public int Id{get;set; }

    public string Name{get;set; } = string.Empty;
}

public class AdminTrackModel
{
    public int Id{get;set; }

    public string Name{get;set; } = string.Empty;

    public string Level{get;set; } = string.Empty;

    public string BatchPrefix{get;set; } = string.Empty;

    public bool IsActive{get;set; }

    public int ModuleCount{get;set; }
}

public class CreateAdminTrackModel
{
    public string Name{get;set; } = string.Empty;

    public string Level{get;set; } = string.Empty;

    public string BatchPrefix{get;set; } = string.Empty;
}

public class AdminBatchModel
{
    public Guid Id{get;set; }

    public string Name{get;set; } = string.Empty;

    public int BatchNumber{get;set; }

    public int TrackId{get;set; }

    public string TrackName{get;set; } = string.Empty;

    public string Level{get;set; } = string.Empty;

    public int Capacity{get;set; }

    public int CurrentStrength{get;set; }

    public bool IsActive{get;set; }

    public Guid? MentorId{get;set; }

    public string MentorName{get;set; } = string.Empty;

    public Guid? AssistantMentorId{get;set; }

    public string AssistantMentorName{get;set; } = string.Empty;
}

public class CreateAdminBatchModel
{
    public string Name{get;set; } = string.Empty;

    public int BatchNumber{get;set; }

    public int TrackId{get;set; }

    public string Level{get;set; } = string.Empty;

    public int Capacity{get;set; } = 15;

    public Guid? MentorId{get;set; }

    public Guid? AssistantMentorId{get;set; }
}

public class UpdateAdminBatchMentorsModel
{
    public Guid? MentorId{get;set; }

    public Guid? AssistantMentorId{get;set; }
}

public class AdminStudentModel
{
    public Guid Id{get;set; }

    public Guid UserId{get;set; }

    public Guid BatchId{get;set; }

    public string FullName{get;set; } = string.Empty;

    public string Email{get;set; } = string.Empty;

    public string PhoneNumber{get;set; } = string.Empty;

    public string CollegeName{get;set; } = string.Empty;

    public string BatchName{get;set; } = string.Empty;

    public string TrackName{get;set; } = string.Empty;

    public DateTime JoinedDate{get;set; }

    public decimal ProgressPercentage{get;set; }

    public decimal AttendancePercentage{get;set; }

    public BillingStatus BillingStatus{get;set; }

    public decimal MonthlyFee{get;set; }

    public DateTime? NextDueDate{get;set; }

    public DateTime? GraceEndsAt{get;set; }

    public bool AutoPayEnabled{get;set; }
}

public class UpdateAdminStudentBillingStatusModel
{
    public BillingStatus Status{get;set; }
}

public class AdminModuleModel
{
    public int Id{get;set; }

    public string Title{get;set; } = string.Empty;

    public string Description{get;set; } = string.Empty;

    public int Order{get;set; }

    public bool IsActive{get;set; }

    public bool IsPublished{get;set; }

    public int LessonCount{get;set; }
}

public class CreateAdminModuleModel
{
    public string Title{get;set; } = string.Empty;

    public string Description{get;set; } = string.Empty;

    public int Order{get;set; }

    public bool IsPublished{get;set; } = true;
}

public class AdminSupportTicketModel
{
    public Guid Id{get;set; }

    public string StudentName{get;set; } = string.Empty;

    public string StudentEmail{get;set; } = string.Empty;

    public string BatchName{get;set; } = string.Empty;

    public string Subject{get;set; } = string.Empty;

    public string Message{get;set; } = string.Empty;

    public SupportTicketStatus Status{get;set; }

    public string MentorResponse{get;set; } = string.Empty;

    public DateTime CreatedAt{get;set; }
}

public class AdminAssignmentSubmissionModel
{
    public Guid Id{get;set; }

    public string AssignmentTitle{get;set; } = string.Empty;

    public string StudentName{get;set; } = string.Empty;

    public string StudentEmail{get;set; } = string.Empty;

    public string BatchName{get;set; } = string.Empty;

    public string SubmissionLink{get;set; } = string.Empty;

    public string Notes{get;set; } = string.Empty;

    public AssignmentSubmissionStatus Status{get;set; }

    public DateTime SubmittedAt{get;set; }

    public string Feedback{get;set; } = string.Empty;
}

public class AdminAttendanceDiscrepancyModel
{
    public Guid Id{get;set; }

    public string StudentName{get;set; } = string.Empty;

    public string StudentEmail{get;set; } = string.Empty;

    public string BatchName{get;set; } = string.Empty;

    public string SessionTitle{get;set; } = string.Empty;

    public AttendanceStatus? CurrentStatus{get;set; }

    public AttendanceStatus RequestedStatus{get;set; }

    public string Reason{get;set; } = string.Empty;

    public AttendanceDiscrepancyStatus Status{get;set; }

    public string ResolutionNote{get;set; } = string.Empty;

    public DateTime CreatedAt{get;set; }
}

public class AdminPaymentModel
{
    public Guid Id{get;set; }

    public string StudentName{get;set; } = string.Empty;

    public string StudentEmail{get;set; } = string.Empty;

    public string BatchName{get;set; } = string.Empty;

    public decimal Amount{get;set; }

    public string Currency{get;set; } = "INR";

    public PaymentStatus Status{get;set; }

    public string Method{get;set; } = string.Empty;

    public string ProviderReference{get;set; } = string.Empty;

    public DateTime DueDate{get;set; }

    public DateTime GraceEndsAt{get;set; }

    public DateTime? PaidAt{get;set; }
}

public class AdminBatchBillingModel
{
    public Guid BatchId{get;set; }

    public string BatchName{get;set; } = string.Empty;

    public string TrackName{get;set; } = string.Empty;

    public int TotalStudents{get;set; }

    public int PaidStudentCount{get;set; }

    public int UnpaidStudentCount{get;set; }

    public decimal PendingAmount{get;set; }

    public List<AdminBatchBillingStudentModel> UnpaidStudents{get;set; }
        = new();
}

public class AdminBatchBillingStudentModel
{
    public Guid StudentId{get;set; }

    public string FullName{get;set; } = string.Empty;

    public string Email{get;set; } = string.Empty;

    public string PhoneNumber{get;set; } = string.Empty;

    public BillingStatus BillingStatus{get;set; }

    public decimal MonthlyFee{get;set; }

    public DateTime? NextDueDate{get;set; }

    public DateTime? GraceEndsAt{get;set; }

    public bool AutoPayEnabled{get;set; }
}
