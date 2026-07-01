using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AxiPlus.Web.Models.Operations;

namespace AxiPlus.Web.Services;

public class OperationsApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public OperationsApiService(HttpClient httpClient, AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
    }

    public Task<MentorProfileModel?> GetMentorProfileAsync()
   {
        return GetAsync<MentorProfileModel>("api/operations/mentor/profile");
    }

    public Task<SalarySlipModel?> CreateSalarySlipAsync(
        CreateSalarySlipModel model)
   {
        return PostAsync<SalarySlipModel>(
            "api/operations/admin/salary-slips",
            model);
    }

    public Task<List<MeetingRequestModel>> GetMentorMeetingRequestsAsync()
   {
        return GetListAsync<MeetingRequestModel>(
            "api/operations/mentor/meeting-requests");
    }

    public Task<MeetingRequestModel?> CreateMeetingRequestAsync(
        CreateMeetingRequestModel model)
   {
        return PostAsync<MeetingRequestModel>(
            "api/operations/mentor/meeting-requests",
            model);
    }

    public Task<MeetingRequestModel?> UpdateMeetingFollowUpAsync(
        Guid id,
        UpdateMeetingFollowUpModel model)
   {
        return PostAsync<MeetingRequestModel>(
            $"api/operations/mentor/meeting-requests/{id}/follow-up",
            model);
    }

    public Task<List<AttendanceDiscrepancyModel>>
        GetMentorAttendanceDiscrepanciesAsync()
   {
        return GetListAsync<AttendanceDiscrepancyModel>(
            "api/operations/mentor/attendance-discrepancies");
    }

    public Task<AttendanceDiscrepancyModel?> ReviewAttendanceDiscrepancyAsync(
        Guid id,
        ReviewAttendanceDiscrepancyModel model)
   {
        return PostAsync<AttendanceDiscrepancyModel>(
            $"api/operations/mentor/attendance-discrepancies/{id}/review",
            model);
    }

    public Task<List<MeetingRequestModel>> GetStudentMeetingRequestsAsync()
   {
        return GetListAsync<MeetingRequestModel>(
            "api/operations/student/meeting-requests");
    }

    public Task<MeetingRequestModel?> RespondToMeetingRequestAsync(
        Guid id,
        RespondMeetingRequestModel model)
   {
        return PostAsync<MeetingRequestModel>(
            $"api/operations/student/meeting-requests/{id}/respond",
            model);
    }

    public Task<List<AttendanceDiscrepancyModel>>
        GetStudentAttendanceDiscrepanciesAsync()
   {
        return GetListAsync<AttendanceDiscrepancyModel>(
            "api/operations/student/attendance-discrepancies");
    }

    public Task<AttendanceDiscrepancyModel?> CreateAttendanceDiscrepancyAsync(
        CreateAttendanceDiscrepancyModel model)
   {
        return PostAsync<AttendanceDiscrepancyModel>(
            "api/operations/student/attendance-discrepancies",
            model);
    }

    private async Task<List<T>> GetListAsync<T>(string url)
   {
        var result = await GetAsync<List<T>>(url);

        return result ?? new List<T>();
    }

    private async Task<T?> GetAsync<T>(string url)
   {
        var request = await CreateRequestAsync(HttpMethod.Get, url);

        return request == null ? default : await SendAsync<T>(request);
    }

    private async Task<T?> PostAsync<T>(string url, object body)
   {
        var request = await CreateRequestAsync(HttpMethod.Post, url);

        if (request == null)
       {
            return default;
        }

        request.Content = JsonContent.Create(body);

        return await SendAsync<T>(request);
    }

    private async Task<T?> SendAsync<T>(HttpRequestMessage request)
   {
        try
       {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
           {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException)
       {
            return default;
        }
        catch (TaskCanceledException)
       {
            return default;
        }
        catch (NotSupportedException)
       {
            return default;
        }
        catch (JsonException)
       {
            return default;
        }
    }

    private async Task<HttpRequestMessage?> CreateRequestAsync(
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
}
