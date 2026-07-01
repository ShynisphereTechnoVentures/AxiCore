using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AxiPlus.Domain.Enums;
using AxiPlus.Web.Models.AdminPortal;

namespace AxiPlus.Web.Services;

public class AdminPortalApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public AdminPortalApiService(
        HttpClient httpClient,
        AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
    }

    public Task<AdminDashboardModel?> GetDashboardAsync()
   {
        return GetAsync<AdminDashboardModel>("api/admin-portal/dashboard");
    }

    public Task<List<AdminRoleModel>> GetRolesAsync()
   {
        return GetListAsync<AdminRoleModel>("api/admin-portal/roles");
    }

    public Task<List<AdminUserModel>> GetUsersAsync()
   {
        return GetListAsync<AdminUserModel>("api/admin-portal/users");
    }

    public Task<AdminUserModel?> CreateUserAsync(CreateAdminUserModel model)
   {
        return PostAsync<AdminUserModel>("api/admin-portal/users", model);
    }

    public Task<AdminUserModel?> UpdateUserStatusAsync(
        Guid userId,
        bool isActive)
   {
        return PostAsync<AdminUserModel>(
            $"api/admin-portal/users/{userId}/status",
            new UpdateUserStatusModel{ IsActive = isActive });
    }

    public Task<List<AdminTrackModel>> GetTracksAsync()
   {
        return GetListAsync<AdminTrackModel>("api/admin-portal/tracks");
    }

    public Task<AdminTrackModel?> CreateTrackAsync(CreateAdminTrackModel model)
   {
        return PostAsync<AdminTrackModel>("api/admin-portal/tracks", model);
    }

    public Task<List<AdminBatchModel>> GetBatchesAsync()
   {
        return GetListAsync<AdminBatchModel>("api/admin-portal/batches");
    }

    public Task<AdminBatchModel?> CreateBatchAsync(CreateAdminBatchModel model)
   {
        return PostAsync<AdminBatchModel>("api/admin-portal/batches", model);
    }

    public Task<AdminBatchModel?> UpdateBatchMentorsAsync(
        Guid batchId,
        UpdateAdminBatchMentorsModel model)
   {
        return PostAsync<AdminBatchModel>(
            $"api/admin-portal/batches/{batchId}/mentors",
            model);
    }

    public Task<List<AdminStudentModel>> GetStudentsAsync()
   {
        return GetListAsync<AdminStudentModel>("api/admin-portal/students");
    }

    public Task<AdminStudentModel?> UpdateStudentBillingStatusAsync(
        Guid studentId,
        BillingStatus status)
   {
        return PostAsync<AdminStudentModel>(
            $"api/admin-portal/students/{studentId}/billing-status",
            new UpdateAdminStudentBillingStatusModel{ Status = status });
    }

    public Task<List<AdminModuleModel>> GetModulesAsync()
   {
        return GetListAsync<AdminModuleModel>("api/admin-portal/modules");
    }

    public Task<AdminModuleModel?> CreateModuleAsync(
        CreateAdminModuleModel model)
   {
        return PostAsync<AdminModuleModel>("api/admin-portal/modules", model);
    }

    public Task<List<AdminSupportTicketModel>> GetSupportTicketsAsync()
   {
        return GetListAsync<AdminSupportTicketModel>(
            "api/admin-portal/support-tickets");
    }

    public Task<List<AdminAssignmentSubmissionModel>>
        GetAssignmentSubmissionsAsync()
   {
        return GetListAsync<AdminAssignmentSubmissionModel>(
            "api/admin-portal/assignment-submissions");
    }

    public Task<List<AdminAttendanceDiscrepancyModel>>
        GetAttendanceDiscrepanciesAsync()
   {
        return GetListAsync<AdminAttendanceDiscrepancyModel>(
            "api/admin-portal/attendance-discrepancies");
    }

    public Task<List<AdminPaymentModel>> GetPaymentsAsync()
   {
        return GetListAsync<AdminPaymentModel>("api/admin-portal/payments");
    }

    public Task<List<AdminBatchBillingModel>> GetBatchBillingAsync()
   {
        return GetListAsync<AdminBatchBillingModel>(
            "api/admin-portal/batch-billing");
    }

    public Task<List<AxiForgeAdminProblemModel>> GetAxiForgeProblemsAsync()
   {
        return GetListAsync<AxiForgeAdminProblemModel>(
            "api/admin-portal/axiforge/problems");
    }

    public Task<AxiForgeAdminProblemModel?> SaveAxiForgeProblemAsync(
        SaveAxiForgeProblemModel model)
   {
        return PostAsync<AxiForgeAdminProblemModel>(
            "api/admin-portal/axiforge/problems",
            model);
    }

    public Task<bool> ArchiveAxiForgeProblemAsync(Guid id)
   {
        return SendAsync(HttpMethod.Post, $"api/admin-portal/axiforge/problems/{id}/archive");
    }

    public Task<bool> DeleteAxiForgeProblemAsync(Guid id)
   {
        return SendAsync(HttpMethod.Delete, $"api/admin-portal/axiforge/problems/{id}");
    }

    public Task<List<AxiForgeAdminRoadmapModel>> GetAxiForgeRoadmapsAsync()
   {
        return GetListAsync<AxiForgeAdminRoadmapModel>(
            "api/admin-portal/axiforge/roadmaps");
    }

    public Task<AxiForgeAdminRoadmapModel?> SaveAxiForgeRoadmapAsync(
        SaveAxiForgeRoadmapModel model)
   {
        return PostAsync<AxiForgeAdminRoadmapModel>(
            "api/admin-portal/axiforge/roadmaps",
            model);
    }

    public Task<bool> ArchiveAxiForgeRoadmapAsync(Guid id)
   {
        return SendAsync(HttpMethod.Post, $"api/admin-portal/axiforge/roadmaps/{id}/archive");
    }

    public Task<bool> DeleteAxiForgeRoadmapAsync(Guid id)
   {
        return SendAsync(HttpMethod.Delete, $"api/admin-portal/axiforge/roadmaps/{id}");
    }

    public Task<List<AxiForgeAdminAssessmentModel>> GetAxiForgeAssessmentsAsync()
   {
        return GetListAsync<AxiForgeAdminAssessmentModel>(
            "api/admin-portal/axiforge/assessments");
    }

    public Task<AxiForgeAdminAssessmentModel?> SaveAxiForgeAssessmentAsync(
        SaveAxiForgeAssessmentModel model)
   {
        return PostAsync<AxiForgeAdminAssessmentModel>(
            "api/admin-portal/axiforge/assessments",
            model);
    }

    public Task<bool> ArchiveAxiForgeAssessmentAsync(Guid id)
   {
        return SendAsync(HttpMethod.Post, $"api/admin-portal/axiforge/assessments/{id}/archive");
    }

    public Task<bool> DeleteAxiForgeAssessmentAsync(Guid id)
   {
        return SendAsync(HttpMethod.Delete, $"api/admin-portal/axiforge/assessments/{id}");
    }

    public Task<List<AxiForgeLessonOptionModel>> GetAxiForgeLessonsAsync()
   {
        return GetListAsync<AxiForgeLessonOptionModel>(
            "api/admin-portal/axiforge/lessons");
    }

    public Task<List<AxiForgeLessonPracticeMappingModel>> GetAxiForgeLessonMappingsAsync()
   {
        return GetListAsync<AxiForgeLessonPracticeMappingModel>(
            "api/admin-portal/axiforge/lesson-mappings");
    }

    public Task<AxiForgeLessonPracticeMappingModel?> SaveAxiForgeLessonMappingAsync(
        SaveAxiForgeLessonPracticeMappingModel model)
   {
        return PostAsync<AxiForgeLessonPracticeMappingModel>(
            "api/admin-portal/axiforge/lesson-mappings",
            model);
    }

    public Task<bool> DeleteAxiForgeLessonMappingAsync(Guid id)
   {
        return SendAsync(HttpMethod.Delete, $"api/admin-portal/axiforge/lesson-mappings/{id}");
    }

    public Task<AxiForgeAdminImportExportModel?> ExportAxiForgeContentAsync()
   {
        return GetAsync<AxiForgeAdminImportExportModel>(
            "api/admin-portal/axiforge/export");
    }

    public Task<bool> ImportAxiForgeContentAsync(AxiForgeAdminImportExportModel model)
   {
        return PostNoContentAsync("api/admin-portal/axiforge/import", model);
    }

    private async Task<List<T>> GetListAsync<T>(string url)
   {
        var result = await GetAsync<List<T>>(url);

        return result ?? new List<T>();
    }

    private async Task<T?> GetAsync<T>(string url)
   {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, url);

        if (request == null)
       {
            return default;
        }

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

    private async Task<T?> PostAsync<T>(string url, object body)
   {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, url);

        if (request == null)
       {
            return default;
        }

        request.Content = JsonContent.Create(body);

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

    private async Task<bool> PostNoContentAsync(string url, object body)
   {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, url);

        if (request == null)
       {
            return false;
        }

        request.Content = JsonContent.Create(body);

        try
       {
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
       {
            return false;
        }
        catch (TaskCanceledException)
       {
            return false;
        }
    }

    private async Task<bool> SendAsync(HttpMethod method, string url)
   {
        var request = await CreateAuthorizedRequestAsync(method, url);

        if (request == null)
       {
            return false;
        }

        try
       {
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
       {
            return false;
        }
        catch (TaskCanceledException)
       {
            return false;
        }
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
}
