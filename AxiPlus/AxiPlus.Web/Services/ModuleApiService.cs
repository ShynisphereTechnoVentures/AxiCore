using System.Net.Http.Json;
using AxiPlus.Web.Models;
using AxiPlus.Web.Models.Modules;

namespace AxiPlus.Web.Services;

public class ModuleApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthorizedApiClient _apiClient;

    public ModuleApiService(
        HttpClient httpClient,
        AuthorizedApiClient apiClient)
   {
        _httpClient = httpClient;
        _apiClient = apiClient;
    }

    public async Task<List<ModuleModel>> GetModulesAsync()
   {
        var result = await _httpClient
            .GetFromJsonAsync<List<ModuleModel>>("api/module");

        return result ?? new List<ModuleModel>();
    }

    public async Task<List<StudentModuleModel>?> GetStudentModulesAsync()
   {
        return await _apiClient.GetListAsync<StudentModuleModel>(
            "api/module/student/me");
    }

    public async Task<ModuleDetailsModel?> GetModuleDetailsAsync(int moduleId)
   {
        return await _apiClient.GetAsync<ModuleDetailsModel>(
            $"api/module/student/me/details/{moduleId}");
    }

}
