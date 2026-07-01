using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.Operations;

public class MentorProfileModel
{
    public Guid UserId{get;set; }
    public string FullName{get;set; } = string.Empty;
    public string Email{get;set; } = string.Empty;
    public string Role{get;set; } = string.Empty;
    public string PhoneNumber{get;set; } = string.Empty;
    public string Address{get;set; } = string.Empty;
    public string EmergencyContact{get;set; } = string.Empty;
    public string Designation{get;set; } = string.Empty;
    public DateTime JoinedDate{get;set; }
    public List<SalarySlipModel> SalarySlips{get;set; } = new();
}

public class SalarySlipModel
{
    public Guid Id{get;set; }
    public Guid UserId{get;set; }
    public string UserName{get;set; } = string.Empty;
    public int Month{get;set; }
    public int Year{get;set; }
    public decimal GrossAmount{get;set; }
    public decimal NetAmount{get;set; }
    public string FileUrl{get;set; } = string.Empty;
}

public class CreateSalarySlipModel
{
    public Guid UserId{get;set; }
    public int Month{get;set; }
    public int Year{get;set; }
    public decimal GrossAmount{get;set; }
    public decimal NetAmount{get;set; }
    public string FileUrl{get;set; } = string.Empty;
}

public class MeetingRequestModel
{
    public Guid Id{get;set; }
    public Guid StudentId{get;set; }
    public string StudentName{get;set; } = string.Empty;
    public string StudentPhoneNumber{get;set; } = string.Empty;
    public Guid MentorUserId{get;set; }
    public string MentorName{get;set; } = string.Empty;
    public string BatchName{get;set; } = string.Empty;
    public DateTime ScheduledAt{get;set; }
    public string MeetingLink{get;set; } = string.Empty;
    public string Reason{get;set; } = string.Empty;
    public MeetingRequestStatus Status{get;set; }
    public string StudentResponseNote{get;set; } = string.Empty;
    public string MentorFollowUpNote{get;set; } = string.Empty;
}

public class CreateMeetingRequestModel
{
    public Guid StudentId{get;set; }
    public DateTime ScheduledAt{get;set; }
    public string MeetingLink{get;set; } = string.Empty;
    public string Reason{get;set; } = string.Empty;
}

public class RespondMeetingRequestModel
{
    public MeetingRequestStatus Status{get;set; }
    public string StudentResponseNote{get;set; } = string.Empty;
}

public class UpdateMeetingFollowUpModel
{
    public MeetingRequestStatus Status{get;set; }
    public string MentorFollowUpNote{get;set; } = string.Empty;
}

public class AttendanceDiscrepancyModel
{
    public Guid Id{get;set; }
    public Guid StudentId{get;set; }
    public string StudentName{get;set; } = string.Empty;
    public Guid SessionId{get;set; }
    public string SessionTitle{get;set; } = string.Empty;
    public AttendanceStatus? CurrentStatus{get;set; }
    public AttendanceStatus RequestedStatus{get;set; }
    public string Reason{get;set; } = string.Empty;
    public AttendanceDiscrepancyStatus Status{get;set; }
    public string ResolutionNote{get;set; } = string.Empty;
}

public class CreateAttendanceDiscrepancyModel
{
    public Guid SessionId{get;set; }
    public AttendanceStatus RequestedStatus{get;set; }
    public string Reason{get;set; } = string.Empty;
}

public class ReviewAttendanceDiscrepancyModel
{
    public AttendanceDiscrepancyStatus Status{get;set; }
    public string ResolutionNote{get;set; } = string.Empty;
}
