using AxiPlus.Application.DTOs.AdminPortal;

namespace AxiPlus.Application.Interfaces;

public interface IAdminPortalService
{       
    Task<AdminDashboardDto> GetDashboardAsync();

    Task<List<AdminRoleDto>> GetRolesAsync();

    Task<List<AdminUserDto>> GetUsersAsync();

    Task<AdminUserDto?> CreateUserAsync(CreateAdminUserDto dto);

    Task<AdminUserDto?> UpdateUserStatusAsync(
        Guid userId,
        UpdateUserStatusDto dto);

    Task<List<AdminTrackDto>> GetTracksAsync();

    Task<AdminTrackDto?> CreateTrackAsync(CreateAdminTrackDto dto);

    Task<List<AdminBatchDto>> GetBatchesAsync();

    Task<AdminBatchDto?> CreateBatchAsync(CreateAdminBatchDto dto);

    Task<AdminBatchDto?> UpdateBatchMentorsAsync(
        Guid batchId,
        UpdateAdminBatchMentorsDto dto);

    Task<List<AdminStudentDto>> GetStudentsAsync();

    Task<AdminStudentDto?> UpdateStudentBillingStatusAsync(
        Guid studentId,
        UpdateAdminStudentBillingStatusDto dto);

    Task<List<AdminModuleDto>> GetModulesAsync();

    Task<AdminModuleDto?> CreateModuleAsync(CreateAdminModuleDto dto);

    Task<List<AdminSupportTicketDto>> GetSupportTicketsAsync();

    Task<List<AdminAssignmentSubmissionDto>> GetAssignmentSubmissionsAsync();

    Task<List<AdminAttendanceDiscrepancyDto>> GetAttendanceDiscrepanciesAsync();

    Task<List<AdminPaymentDto>> GetPaymentsAsync();

    Task<List<AdminBatchBillingDto>> GetBatchBillingAsync();
}
