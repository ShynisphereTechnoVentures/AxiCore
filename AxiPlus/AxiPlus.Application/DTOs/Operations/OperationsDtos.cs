using AxiPlus.Domain.Enums;

namespace AxiPlus.Application.DTOs.Operations;

public class MentorProfileDto
{
    public Guid UserId{get; set; }

    public string FullName{get; set; } = string.Empty;

    public string Email{get; set; } = string.Empty;

    public string Role{get; set; } = string.Empty;

    public string PhoneNumber{get; set; } = string.Empty;

    public string Address{get; set; } = string.Empty;

    public string EmergencyContact{get; set; } = string.Empty;

    public string Designation{get; set; } = string.Empty;

    public DateTime JoinedDate{get; set; }

    public List<SalarySlipDto> SalarySlips{get; set; } = new();
}

public class SalarySlipDto
{
    public Guid Id{get; set; }

    public Guid UserId{get; set; }

    public string UserName{get; set; } = string.Empty;

    public int Month{get; set; }

    public int Year{get; set; }

    public decimal GrossAmount{get; set; }

    public decimal NetAmount{get; set; }

    public string FileUrl{get; set; } = string.Empty;
}

public class CreateSalarySlipDto
{
    public Guid UserId{get; set; }

    public int Month{get; set; }

    public int Year{get; set; }

    public decimal GrossAmount{get; set; }

    public decimal NetAmount{get; set; }

    public string FileUrl{get; set; } = string.Empty;
}

public class MeetingRequestDto
{
    public Guid Id{get; set; }

    public Guid StudentId{get; set; }

    public string StudentName{get; set; } = string.Empty;

    public string StudentPhoneNumber{get; set; } = string.Empty;

    public Guid MentorUserId{get; set; }

    public string MentorName{get; set; } = string.Empty;

    public string BatchName{get; set; } = string.Empty;

    public DateTime ScheduledAt{get; set; }

    public string MeetingLink{get; set; } = string.Empty;

    public string Reason{get; set; } = string.Empty;

    public MeetingRequestStatus Status{get; set; }

    public string StudentResponseNote{get; set; } = string.Empty;

    public string MentorFollowUpNote{get; set; } = string.Empty;
}

public class CreateMeetingRequestDto
{
    public Guid StudentId{get; set; }

    public DateTime ScheduledAt{get; set; }

    public string MeetingLink{get; set; } = string.Empty;

    public string Reason{get; set; } = string.Empty;
}

public class RespondMeetingRequestDto
{
    public MeetingRequestStatus Status{get; set; }

    public string StudentResponseNote{get; set; } = string.Empty;
}

public class UpdateMeetingFollowUpDto
{
    public MeetingRequestStatus Status{get; set; }

    public string MentorFollowUpNote{get; set; } = string.Empty;
}

public class AttendanceDiscrepancyDto
{
    public Guid Id{get; set; }

    public Guid StudentId{get; set; }

    public string StudentName{get; set; } = string.Empty;

    public Guid SessionId{get; set; }

    public string SessionTitle{get; set; } = string.Empty;

    public AttendanceStatus? CurrentStatus{get; set; }

    public AttendanceStatus RequestedStatus{get; set; }

    public string Reason{get; set; } = string.Empty;

    public AttendanceDiscrepancyStatus Status{get; set; }

    public string ResolutionNote{get; set; } = string.Empty;
}

public class CreateAttendanceDiscrepancyDto
{
    public Guid SessionId{get; set; }

    public AttendanceStatus RequestedStatus{get; set; }

    public string Reason{get; set; } = string.Empty;
}

public class ReviewAttendanceDiscrepancyDto
{
    public AttendanceDiscrepancyStatus Status{get; set; }

    public string ResolutionNote{get; set; } = string.Empty;
}
