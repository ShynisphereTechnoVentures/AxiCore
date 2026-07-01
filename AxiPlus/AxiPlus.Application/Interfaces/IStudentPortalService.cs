using AxiPlus.Application.DTOs.StudentPortal;

namespace AxiPlus.Application.Interfaces;

public interface IStudentPortalService
{        
    Task<List<StudentLiveClassDto>> GetLiveClassesAsync(string email);

    Task<List<StudentRecordingDto>> GetRecordingsAsync(string email);

    Task<List<StudentPracticeItemDto>> GetPracticeAsync(string email);

    Task<List<StudentNotificationDto>> GetNotificationsAsync(string email);

    Task<bool> MarkNotificationReadAsync(string email, Guid notificationId);

    Task<List<SupportTicketDto>> GetSupportTicketsAsync(string email);

    Task<SupportTicketDto?> CreateSupportTicketAsync(
        string email,
        CreateSupportTicketDto dto);
}
