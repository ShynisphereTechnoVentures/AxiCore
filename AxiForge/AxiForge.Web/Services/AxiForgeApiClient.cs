using System.Net.Http.Headers;
using System.Net.Http.Json;
using AxiCore.Diagnostics;
using AxiForge.Application.DTOs.Assessments;
using AxiForge.Application.DTOs.Auth;
using AxiForge.Application.DTOs.Coding;
using AxiForge.Application.DTOs.Dashboard;
using AxiForge.Application.DTOs.Launch;
using AxiForge.Application.DTOs.Roadmaps;

namespace AxiForge.Web.Services;

public sealed class AxiForgeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;
    private readonly ILogger<AxiForgeApiClient> _logger;

    public AxiForgeApiClient(
        HttpClient httpClient,
        AuthStateService authState,
        ILogger<AxiForgeApiClient> logger)
    {
        _httpClient = httpClient;
        _authState = authState;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new AxiForge student account through the API.
    /// Returns authentication details so the web shell can enter the student dashboard immediately.
    /// </summary>
    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(RegisterAsync));
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Authenticates an AxiForge user through the API.
    /// Returns authentication details so authorized student APIs can be called.
    /// </summary>
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(LoginAsync));
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Requests a password reset for an AxiForge account.
    /// Returns true when the API accepts the request so the UI can show safe reset feedback.
    /// </summary>
    public async Task<bool> RequestPasswordResetAsync(PasswordResetRequestDto request)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(RequestPasswordResetAsync));
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/request-password-reset", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return false;
        }
    }

    /// <summary>
    /// Exchanges an AxiPlus practice launch token for an AxiForge session.
    /// Returns authentication details so launched child accounts can continue without separate manual login.
    /// </summary>
    public async Task<AuthResponseDto?> LaunchLoginAsync(string token)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(LaunchLoginAsync));
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var response = await _httpClient.PostAsync(
                $"api/auth/launch-login?token={Uri.EscapeDataString(token)}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Gets the current student's dashboard from the AxiForge API.
    /// Returns dashboard data when the active circuit has a valid token.
    /// </summary>
    public async Task<StudentDashboardDto?> GetDashboardAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetDashboardAsync));
        try
        {
            if (string.IsNullOrWhiteSpace(_authState.Token))
            {
                return null;
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/dashboard/student/me");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<StudentDashboardDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Validates an AxiPlus-to-AxiForge launch token through the API.
    /// Returns launch context so the web app can continue the student into the right practice flow.
    /// </summary>
    public async Task<LaunchValidationResponseDto?> ValidateLaunchAsync(string token)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(ValidateLaunchAsync));
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            return await _httpClient.GetFromJsonAsync<LaunchValidationResponseDto>(
                $"api/practice/launch?token={Uri.EscapeDataString(token)}");
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Gets the AxiForge problem bank.
    /// Returns problem summaries so the practice page can list available exercises.
    /// </summary>
    public async Task<List<CodingProblemSummaryDto>> GetProblemsAsync(
        string? topic = null,
        string? difficulty = null,
        string? search = null)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetProblemsAsync));
        try
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(topic))
            {
                query.Add($"topic={Uri.EscapeDataString(topic)}");
            }

            if (!string.IsNullOrWhiteSpace(difficulty))
            {
                query.Add($"difficulty={Uri.EscapeDataString(difficulty)}");
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query.Add($"search={Uri.EscapeDataString(search)}");
            }

            var queryString = string.Join("&", query);
            var uri = string.IsNullOrWhiteSpace(queryString)
                ? "api/coding/problems"
                : $"api/coding/problems?{queryString}";

            return await _httpClient.GetFromJsonAsync<List<CodingProblemSummaryDto>>(uri)
                ?? new List<CodingProblemSummaryDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return new List<CodingProblemSummaryDto>();
        }
    }

    /// <summary>
    /// Gets a coding problem by identifier.
    /// Returns full problem details for the practice editor.
    /// </summary>
    public async Task<CodingProblemDetailDto?> GetProblemAsync(Guid problemId)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetProblemAsync));
        try
        {
            return await _httpClient.GetFromJsonAsync<CodingProblemDetailDto>(
                $"api/coding/problems/{problemId}");
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Submits code for evaluation through the AxiForge API.
    /// Returns the submission result so the practice page can show feedback.
    /// </summary>
    public async Task<CodingSubmissionDto?> SubmitAsync(CreateSubmissionRequestDto submission)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(SubmitAsync));
        try
        {
            if (string.IsNullOrWhiteSpace(_authState.Token))
            {
                return null;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/coding/submissions");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
            request.Content = JsonContent.Create(submission);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CodingSubmissionDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    /// <summary>
    /// Gets the current student's submission history.
    /// Returns recent submissions for the dashboard and practice review surfaces.
    /// </summary>
    public async Task<List<CodingSubmissionDto>> GetMySubmissionsAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetMySubmissionsAsync));
        try
        {
            if (string.IsNullOrWhiteSpace(_authState.Token))
            {
                return new List<CodingSubmissionDto>();
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/coding/submissions/me");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new List<CodingSubmissionDto>();
            }

            return await response.Content.ReadFromJsonAsync<List<CodingSubmissionDto>>()
                ?? new List<CodingSubmissionDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return new List<CodingSubmissionDto>();
        }
    }

    public async Task<List<RoadmapTemplateDto>> GetRoadmapsAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetRoadmapsAsync));
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RoadmapTemplateDto>>("api/roadmaps")
                ?? new List<RoadmapTemplateDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return new List<RoadmapTemplateDto>();
        }
    }

    /// <summary>
    /// Gets a roadmap template with its published steps.
    /// Returns roadmap detail data for the study plan page.
    /// </summary>
    public async Task<RoadmapDetailDto?> GetRoadmapAsync(Guid roadmapId)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetRoadmapAsync));
        try
        {
            return await _httpClient.GetFromJsonAsync<RoadmapDetailDto>($"api/roadmaps/{roadmapId}");
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    public async Task<StudentRoadmapDto?> EnrollRoadmapAsync(Guid roadmapId)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(EnrollRoadmapAsync));
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Post, $"api/roadmaps/{roadmapId}/enroll");
            if (request == null)
            {
                return null;
            }

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<StudentRoadmapDto>()
                : null;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    public async Task<List<StudentRoadmapDto>> GetMyRoadmapsAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetMyRoadmapsAsync));
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, "api/roadmaps/me");
            if (request == null)
            {
                return new List<StudentRoadmapDto>();
            }

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<StudentRoadmapDto>>() ?? new List<StudentRoadmapDto>()
                : new List<StudentRoadmapDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return new List<StudentRoadmapDto>();
        }
    }

    public async Task<List<AssessmentTemplateDto>> GetAssessmentsAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetAssessmentsAsync));
        try
        {
            return await _httpClient.GetFromJsonAsync<List<AssessmentTemplateDto>>("api/assessments")
                ?? new List<AssessmentTemplateDto>();
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return new List<AssessmentTemplateDto>();
        }
    }

    public async Task<AssessmentDetailDto?> GetAssessmentAsync(Guid assessmentId)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(GetAssessmentAsync));
        try
        {
            return await _httpClient.GetFromJsonAsync<AssessmentDetailDto>($"api/assessments/{assessmentId}");
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    public async Task<AssessmentResultDto?> SubmitAssessmentAsync(SubmitAssessmentRequestDto submission)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeApiClient), nameof(SubmitAssessmentAsync));
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Post, "api/assessments/submit");
            if (request == null)
            {
                return null;
            }

            request.Content = JsonContent.Create(submission);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<AssessmentResultDto>()
                : null;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return null;
        }
    }

    private HttpRequestMessage? CreateAuthorizedRequest(HttpMethod method, string url)
    {
        Console.WriteLine("Entering -> AxiForgeApiClient.cs -> CreateAuthorizedRequest");
        try
        {
            if (string.IsNullOrWhiteSpace(_authState.Token))
            {
                return null;
            }

            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authState.Token);
            return request;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiForgeApiClient.cs -> CreateAuthorizedRequest -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiForgeApiClient.cs -> CreateAuthorizedRequest");
        }
    }
}
