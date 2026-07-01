using AxiPlus.Application.DTOs.MentorPortal;

namespace AxiPlus.Application.Interfaces;

public interface IMentorPortalService
{       
    Task<MentorDashboardDto?> GetDashboardAsync(Guid mentorUserId);

    Task<List<MentorBatchDto>> GetBatchesAsync(Guid mentorUserId);

    Task<List<MentorStudentDto>> GetStudentsAsync(Guid mentorUserId);

    Task<List<MentorLessonOptionDto>> GetLessonOptionsAsync(Guid mentorUserId);

    Task<List<MentorLiveClassDto>> GetLiveClassesAsync(Guid mentorUserId);

    Task<MentorLiveClassDto?> CreateLiveClassAsync(
        Guid mentorUserId,
        CreateMentorLiveClassDto dto);

    Task<bool> DeleteLiveClassAsync(Guid mentorUserId, Guid liveClassId);

    Task<List<MentorAssignmentDto>> GetAssignmentsAsync(Guid mentorUserId);

    Task<MentorAssignmentDto?> CreateAssignmentAsync(
        Guid mentorUserId,
        CreateMentorAssignmentDto dto);

    Task<bool> DeleteAssignmentAsync(Guid mentorUserId, Guid assignmentId);

    Task<List<MentorSubmissionDto>> GetSubmissionsAsync(Guid mentorUserId);

    Task<MentorSubmissionDto?> ReviewSubmissionAsync(
        Guid mentorUserId,
        Guid submissionId,
        ReviewAssignmentSubmissionDto dto);

    Task<List<MentorSessionDto>> GetSessionsAsync(Guid mentorUserId);

    Task<MentorSessionDto?> CreateSessionAsync(
        Guid mentorUserId,
        CreateMentorSessionDto dto);

    Task<List<MentorSessionDto>> CreateWeeklySessionsAsync(
        Guid mentorUserId,
        CreateMentorWeeklySessionsDto dto);

    Task<bool> DeleteSessionAsync(Guid mentorUserId, Guid sessionId);

    Task<MentorAttendanceRosterDto?> GetAttendanceRosterAsync(
        Guid mentorUserId,
        Guid sessionId);

    Task<MentorAttendanceRosterDto?> MarkAttendanceAsync(
        Guid mentorUserId,
        Guid sessionId,
        MarkMentorAttendanceDto dto);

    Task<List<MentorStudentReportDto>> GetStudentReportsAsync(Guid mentorUserId);

    Task<List<MentorSupportTicketDto>> GetSupportTicketsAsync(Guid mentorUserId);

    Task<MentorSupportTicketDto?> RespondToSupportTicketAsync(
        Guid mentorUserId,
        Guid ticketId,
        RespondSupportTicketDto dto);
}
