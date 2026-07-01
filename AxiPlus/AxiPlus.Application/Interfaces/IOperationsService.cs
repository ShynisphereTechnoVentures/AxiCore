using AxiPlus.Application.DTOs.Operations;

namespace AxiPlus.Application.Interfaces;

public interface IOperationsService
{       
    Task<MentorProfileDto?> GetMentorProfileAsync(Guid userId);

    Task<List<SalarySlipDto>> GetSalarySlipsAsync(Guid userId);

    Task<SalarySlipDto?> CreateSalarySlipAsync(
        Guid adminUserId,
        CreateSalarySlipDto dto);

    Task<List<MeetingRequestDto>> GetMentorMeetingRequestsAsync(
        Guid mentorUserId);

    Task<List<MeetingRequestDto>> GetStudentMeetingRequestsAsync(
        Guid studentUserId);

    Task<MeetingRequestDto?> CreateMeetingRequestAsync(
        Guid mentorUserId,
        CreateMeetingRequestDto dto);

    Task<MeetingRequestDto?> RespondToMeetingRequestAsync(
        Guid studentUserId,
        Guid meetingRequestId,
        RespondMeetingRequestDto dto);

    Task<MeetingRequestDto?> UpdateMeetingFollowUpAsync(
        Guid mentorUserId,
        Guid meetingRequestId,
        UpdateMeetingFollowUpDto dto);

    Task<List<AttendanceDiscrepancyDto>> GetStudentAttendanceDiscrepanciesAsync(
        Guid studentUserId);

    Task<List<AttendanceDiscrepancyDto>> GetMentorAttendanceDiscrepanciesAsync(
        Guid mentorUserId);

    Task<AttendanceDiscrepancyDto?> CreateAttendanceDiscrepancyAsync(
        Guid studentUserId,
        CreateAttendanceDiscrepancyDto dto);

    Task<AttendanceDiscrepancyDto?> ReviewAttendanceDiscrepancyAsync(
        Guid mentorUserId,
        Guid discrepancyId,
        ReviewAttendanceDiscrepancyDto dto);
}
