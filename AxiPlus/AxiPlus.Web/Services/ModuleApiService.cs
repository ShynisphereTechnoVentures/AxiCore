using System.Net.Http.Headers;
using System.Net.Http.Json;
using AxiPlus.Web.Models;
using AxiPlus.Web.Models.Modules;

namespace AxiPlus.Web.Services;

public class ModuleApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ModuleApiService(
        HttpClient httpClient,
        AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<ModuleModel>> GetModulesAsync()
   {
        var result = await _httpClient
            .GetFromJsonAsync<List<ModuleModel>>("api/module");

        return result ?? new List<ModuleModel>();
    }

    public async Task<List<StudentModuleModel>?> GetStudentModulesAsync()
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            "api/module/student/me");

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
            .ReadFromJsonAsync<List<StudentModuleModel>>()
            ?? new List<StudentModuleModel>();
    }

    public async Task<ModuleDetailsModel?> GetModuleDetailsAsync(int moduleId)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            $"api/module/student/me/details/{moduleId}");

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
            .ReadFromJsonAsync<ModuleDetailsModel>()
            ?? new ModuleDetailsModel();
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
