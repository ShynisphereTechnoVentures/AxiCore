using System.Net.Http.Headers;
using AxiPlus.Web.Models.Attendance;

namespace AxiPlus.Web.Services;

public class AttendanceApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public AttendanceApiService(
        HttpClient httpClient,
        AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<StudentAttendanceSummaryModel?>
        GetMineAsync()
   {
        var token = await _authService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
       {
            return null;
        }

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/attendance/me");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<StudentAttendanceSummaryModel>();
    }
}
