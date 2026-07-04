using System.Net.Http.Json;
using AxiPlus.Web.Models;
using AxiPlus.Web.Models.Modules;

namespace AxiPlus.Web.Services;

public class LessonApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthorizedApiClient _apiClient;

    public LessonApiService(
        HttpClient httpClient,
        AuthorizedApiClient apiClient)
   {
        _httpClient = httpClient;
        _apiClient = apiClient;
    }

    public async Task<List<LessonModel>> GetLessonsByModuleAsync(int moduleId)
   {
        var result = await _httpClient
            .GetFromJsonAsync<List<LessonModel>>(
                $"api/lesson/module/{moduleId}");

        return result ?? new List<LessonModel>();
    }

    public async Task<LessonModel?> GetByIdAsync(Guid id)
   {
        return await _httpClient
            .GetFromJsonAsync<LessonModel>($"api/lesson/{id}");
    }

    public async Task<LessonDetailsModel?> GetLessonDetailsAsync(Guid lessonId)
   {
        return await _apiClient.GetAsync<LessonDetailsModel>(
            $"api/lesson/student/me/details/{lessonId}");
    }

    public async Task StartLessonAsync(Guid lessonId)
   {
        await _apiClient.SendAsync(
            HttpMethod.Post,
            $"api/lesson/student/me/start/{lessonId}");
    }

    public async Task CompleteLessonAsync(Guid lessonId)
   {
        await _apiClient.SendAsync(
            HttpMethod.Post,
            $"api/lesson/student/me/complete/{lessonId}");
    }

}
