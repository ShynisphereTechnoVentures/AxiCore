using System.Net.Http.Headers;
using AxiPlus.Web.Models.StudentPortal;

namespace AxiPlus.Web.Services;

public class StudentPortalApiService
{
    private readonly AuthorizedApiClient _apiClient;

    public StudentPortalApiService(AuthorizedApiClient apiClient)
   {
        _apiClient = apiClient;
    }

    public Task<List<StudentLiveClassModel>> GetLiveClassesAsync()
   {
        return GetListAsync<StudentLiveClassModel>(
            "api/student-portal/live-classes");
    }

    public Task<List<StudentRecordingModel>> GetRecordingsAsync()
   {
        return GetListAsync<StudentRecordingModel>(
            "api/student-portal/recordings");
    }

    public Task<List<StudentPracticeItemModel>> GetPracticeAsync()
   {
        return GetListAsync<StudentPracticeItemModel>(
            "api/student-portal/practice");
    }

    public async Task<string?> CreatePracticeLaunchUrlAsync(Guid lessonId)
   {
        var launch = await _apiClient.PostAsync<PracticeLaunchResponseModel>(
            $"api/practice-launch/lessons/{lessonId}",
            new { });
        return launch?.RedirectUrl;
    }

    public Task<List<StudentNotificationModel>> GetNotificationsAsync()
   {
        return GetListAsync<StudentNotificationModel>(
            "api/student-portal/notifications");
    }

    public async Task<bool> MarkNotificationReadAsync(Guid id)
   {
        return await _apiClient.SendAsync(
            HttpMethod.Post,
            $"api/student-portal/notifications/{id}/read");
    }

    public Task<List<SupportTicketModel>> GetSupportTicketsAsync()
   {
        return GetListAsync<SupportTicketModel>(
            "api/student-portal/support-tickets");
    }

    public async Task<SupportTicketModel?> CreateSupportTicketAsync(
        CreateSupportTicketModel model)
   {
        return await _apiClient.PostAsync<SupportTicketModel>(
            "api/student-portal/support-tickets",
            model);
    }

    private async Task<List<T>> GetListAsync<T>(string url)
   {
        return await _apiClient.GetListAsync<T>(url);
    }

    private sealed class PracticeLaunchResponseModel
    {
        public string RedirectUrl{ get; set; } = string.Empty;
    }
}
