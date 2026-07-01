using AxiPlus.Domain.Enums;

namespace AxiPlus.Application.DTOs.MentorPortal;

public class MentorDashboardDto
{
    public string MentorName{ get; set; } = string.Empty;

    public string Role{ get; set; } = string.Empty;

    public int AssignedBatchCount{ get; set; }

    public int StudentCount{ get; set; }

    public int UpcomingClassCount{ get; set; }

    public int PendingSubmissionCount{ get; set; }

    public int OpenSupportTicketCount{ get; set; }

    public int AttendanceSessionCount{ get; set; }
}

public class MentorBatchDto
{
    public Guid BatchId{ get; set; }

    public string Name{ get; set; } = string.Empty;

    public string TrackName{ get; set; } = string.Empty;

    public string Level{ get; set; } = string.Empty;

    public int CurrentStrength{ get; set; }

    public int Capacity{ get; set; }

    public decimal AverageProgressPercentage{ get; set; }

    public decimal AttendancePercentage{ get; set; }
}

public class MentorStudentDto
{
    public Guid StudentId{ get; set; }

    public Guid UserId{ get; set; }

    public Guid BatchId{ get; set; }

    public string FullName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public string TrackName{ get; set; } = string.Empty;

    public decimal AttendancePercentage{ get; set; }

    public decimal ProgressPercentage{ get; set; }

    public int PendingAssignments{ get; set; }

    public int CompletedLessons{ get; set; }

    public int TotalLessons{ get; set; }
}

public class MentorLessonOptionDto
{
    public Guid LessonId{ get; set; }

    public int ModuleId{ get; set; }

    public string ModuleTitle{ get; set; } = string.Empty;

    public string LessonTitle{ get; set; } = string.Empty;
}

public class MentorLiveClassDto
{
    public Guid Id{ get; set; }

    public Guid LessonId{ get; set; }

    public string LessonTitle{ get; set; } = string.Empty;

    public string ModuleTitle{ get; set; } = string.Empty;

    public DateTime ScheduledAt{ get; set; }

    public string MeetingLink{ get; set; } = string.Empty;

    public string RecordingLink{ get; set; } = string.Empty;

    public bool IsCompleted{ get; set; }
}

public class CreateMentorLiveClassDto
{
    public Guid LessonId{ get; set; }

    public DateTime ScheduledAt{ get; set; }

    public string MeetingLink{ get; set; } = string.Empty;

    public string RecordingLink{ get; set; } = string.Empty;
}

public class MentorAssignmentDto
{
    public Guid AssignmentId{ get; set; }

    public Guid BatchId{ get; set; }

    public string BatchName{ get; set; } = string.Empty;

    public Guid? LessonId{ get; set; }

    public string LessonTitle{ get; set; } = string.Empty;

    public string ModuleTitle{ get; set; } = string.Empty;

    public string Title{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public DateTime DueAt{ get; set; }

    public bool IsPublished{ get; set; }

    public bool IsOverdue{ get; set; }

    public string CreatedByName{ get; set; } = string.Empty;

    public string SourceRole{ get; set; } = string.Empty;

    public int SubmissionCount{ get; set; }

    public int PendingReviewCount{ get; set; }
}

public class CreateMentorAssignmentDto
{
    public Guid BatchId{ get; set; }

    public Guid? LessonId{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public DateTime DueAt{ get; set; }

    public bool IsPublished{ get; set; } = true;
}

public class MentorSubmissionDto
{
    public Guid SubmissionId{ get; set; }

    public Guid AssignmentId{ get; set; }

    public Guid BatchId{ get; set; }

    public string AssignmentTitle{ get; set; } = string.Empty;

    public string StudentName{ get; set; } = string.Empty;

    public string StudentEmail{ get; set; } = string.Empty;

    public string SubmissionLink{ get; set; } = string.Empty;

    public string Notes{ get; set; } = string.Empty;

    public AssignmentSubmissionStatus Status{ get; set; }

    public DateTime SubmittedAt{ get; set; }

    public string Feedback{ get; set; } = string.Empty;
}

public class ReviewAssignmentSubmissionDto
{
    public AssignmentSubmissionStatus Status{ get; set; }

    public string Feedback{ get; set; } = string.Empty;
}

public class MentorSessionDto
{
    public Guid SessionId{ get; set; }

    public Guid BatchId{ get; set; }

    public string BatchName{ get; set; } = string.Empty;

    public string Title{ get; set; } = string.Empty;

    public string MeetLink{ get; set; } = string.Empty;

    public DateTime StartTime{ get; set; }

    public DateTime EndTime{ get; set; }

    public int StudentCount{ get; set; }

    public int MarkedCount{ get; set; }

    public int PresentCount{ get; set; }

    public int LateCount{ get; set; }

    public int AbsentCount{ get; set; }
}

public class CreateMentorSessionDto
{
    public Guid BatchId{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string MeetLink{ get; set; } = string.Empty;

    public DateTime StartTime{ get; set; }

    public DateTime EndTime{ get; set; }
}

public class CreateMentorWeeklySessionsDto
{
    public Guid BatchId{ get; set; }

    public DateTime WeekStartDate{ get; set; }

    public string TitlePrefix{ get; set; } = "Live Class";

    public string MeetLink{ get; set; } = string.Empty;

    public TimeSpan StartTime{ get; set; }

    public TimeSpan EndTime{ get; set; }

    public List<DayOfWeek> Days{ get; set; } = new();
}

public class MentorStudentReportDto
{
    public Guid StudentId{ get; set; }

    public Guid BatchId{ get; set; }

    public string StudentName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public string BatchName{ get; set; } = string.Empty;

    public string TrackName{ get; set; } = string.Empty;

    public decimal AttendancePercentage{ get; set; }

    public int AttendanceMarkedCount{ get; set; }

    public int PresentCount{ get; set; }

    public int LateCount{ get; set; }

    public int AbsentCount{ get; set; }

    public int AssignmentCount{ get; set; }

    public int SubmittedAssignmentCount{ get; set; }

    public int ReviewedAssignmentCount{ get; set; }

    public int PendingAssignmentCount{ get; set; }

    public int ProjectCount{ get; set; }

    public int SubmittedProjectCount{ get; set; }

    public int ReviewedProjectCount{ get; set; }

    public int ExamPassedCount{ get; set; }

    public int ExamPendingCount{ get; set; }

    public int ExamAttempts{ get; set; }

    public decimal ProgressPercentage{ get; set; }
}

public class MentorAttendanceRosterDto
{
    public Guid SessionId{ get; set; }

    public string SessionTitle{ get; set; } = string.Empty;

    public List<MentorAttendanceEntryDto> Students{ get; set; } = new();
}

public class MentorAttendanceEntryDto
{
    public Guid StudentId{ get; set; }

    public string StudentName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public AttendanceStatus? Status{ get; set; }
}

public class MarkMentorAttendanceDto
{
    public List<MarkMentorAttendanceEntryDto> Records{ get; set; } = new();
}

public class MarkMentorAttendanceEntryDto
{
    public Guid StudentId{ get; set; }

    public AttendanceStatus Status{ get; set; }
}

public class MentorSupportTicketDto
{
    public Guid Id{ get; set; }

    public string StudentName{ get; set; } = string.Empty;

    public string StudentEmail{ get; set; } = string.Empty;

    public string StudentPhoneNumber{ get; set; } = string.Empty;

    public Guid BatchId{ get; set; }

    public string BatchName{ get; set; } = string.Empty;

    public string Subject{ get; set; } = string.Empty;

    public string Message{ get; set; } = string.Empty;

    public SupportTicketStatus Status{ get; set; }

    public string MentorResponse{ get; set; } = string.Empty;

    public DateTime CreatedAt{ get; set; }

    public DateTime? UpdatedAt{ get; set; }
}

public class RespondSupportTicketDto
{
    public SupportTicketStatus Status{ get; set; }

    public string MentorResponse{ get; set; } = string.Empty;
}
