using System.Net.Http.Headers;
using System.Net.Http.Json;
using AxiPlus.Web.Models;
using AxiPlus.Web.Models.Modules;

namespace AxiPlus.Web.Services;

public class LessonApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public LessonApiService(
        HttpClient httpClient,
        AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
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
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            $"api/lesson/student/me/details/{lessonId}");

        if (request == null)
       {
            return null;
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<LessonDetailsModel>()
            ?? new LessonDetailsModel();
    }

    public async Task StartLessonAsync(Guid lessonId)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            $"api/lesson/student/me/start/{lessonId}");

        if (request == null)
       {
            return;
        }

        await _httpClient.SendAsync(request);
    }

    public async Task CompleteLessonAsync(Guid lessonId)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            $"api/lesson/student/me/complete/{lessonId}");

        if (request == null)
       {
            return;
        }

        await _httpClient.SendAsync(request);
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
