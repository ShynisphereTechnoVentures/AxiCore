using AxiCore.Diagnostics;
using AxiPlus.Web.Models;

namespace AxiPlus.Web.Services;

public class DashboardApiService
{
    private readonly AuthorizedApiClient _apiClient;
    private readonly ILogger<DashboardApiService> _logger;

    public DashboardApiService(
        AuthorizedApiClient apiClient,
        ILogger<DashboardApiService> logger)
    {
        _apiClient = apiClient;
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
            return await _apiClient.GetAsync<StudentDashboardModel>(
                "api/dashboard/student/me");
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

}
