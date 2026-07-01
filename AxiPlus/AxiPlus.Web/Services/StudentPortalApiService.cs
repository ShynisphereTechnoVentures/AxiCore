using System.Net.Http.Headers;
using AxiPlus.Web.Models.StudentPortal;

namespace AxiPlus.Web.Services;

public class StudentPortalApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public StudentPortalApiService(
        HttpClient httpClient,
        AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
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
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            $"api/practice-launch/lessons/{lessonId}");

        if (request == null)
       {
            return null;
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return null;
        }

        var launch = await response.Content.ReadFromJsonAsync<PracticeLaunchResponseModel>();
        return launch?.RedirectUrl;
    }

    public Task<List<StudentNotificationModel>> GetNotificationsAsync()
   {
        return GetListAsync<StudentNotificationModel>(
            "api/student-portal/notifications");
    }

    public async Task<bool> MarkNotificationReadAsync(Guid id)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            $"api/student-portal/notifications/{id}/read");

        if (request == null)
       {
            return false;
        }

        var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    public Task<List<SupportTicketModel>> GetSupportTicketsAsync()
   {
        return GetListAsync<SupportTicketModel>(
            "api/student-portal/support-tickets");
    }

    public async Task<SupportTicketModel?> CreateSupportTicketAsync(
        CreateSupportTicketModel model)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            "api/student-portal/support-tickets");

        if (request == null)
       {
            return null;
        }

        request.Content = JsonContent.Create(model);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<SupportTicketModel>();
    }

    private async Task<List<T>> GetListAsync<T>(string url)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            url);

        if (request == null)
       {
            return new List<T>();
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return new List<T>();
        }

        return await response.Content
            .ReadFromJsonAsync<List<T>>()
            ?? new List<T>();
    }

    private async Task<HttpRequestMessage?> CreateAuthorizedRequestAsync(
        HttpMethod method,
        string url)
   {
        var token = await _authService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
       {
            return null;
        }

        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return request;
    }

    private sealed class PracticeLaunchResponseModel
    {
        public string RedirectUrl{ get; set; } = string.Empty;
    }
}
