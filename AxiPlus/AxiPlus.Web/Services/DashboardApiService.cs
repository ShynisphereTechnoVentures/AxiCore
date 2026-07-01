using System.Net.Http.Headers;
using System.Net.Http.Json;
using AxiCore.Diagnostics;
using AxiPlus.Web.Models;

namespace AxiPlus.Web.Services;

public class DashboardApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ILogger<DashboardApiService> _logger;

    public DashboardApiService(
        HttpClient httpClient,
        AuthService authService,
        ILogger<DashboardApiService> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the authenticated student's dashboard from the AxiPlus API.
    /// Returns dashboard data when the token is valid so the student landing page can render without redirect loops.
    /// </summary>
    public async Task<StudentDashboardModel?> GetStudentDashboardAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(DashboardApiService), nameof(GetStudentDashboardAsync));
        try
        {
            var request = await CreateAuthorizedRequestAsync(
                HttpMethod.Get,
                "api/dashboard/student/me");

            if (request == null)
            {
                return null;
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Student dashboard API failed with status {StatusCode}",
                    response.StatusCode);
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<StudentDashboardModel>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Creates an authorized API request using the browser-stored JWT.
    /// Returns null when the user has no token so callers can avoid unauthenticated API calls.
    /// </summary>
    private async Task<HttpRequestMessage?> CreateAuthorizedRequestAsync(
        HttpMethod method,
        string url)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(DashboardApiService), nameof(CreateAuthorizedRequestAsync));
        try
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
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
