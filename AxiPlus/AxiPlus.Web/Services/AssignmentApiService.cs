using System.Net.Http.Headers;
using AxiPlus.Web.Models.Assignments;

namespace AxiPlus.Web.Services;

public class AssignmentApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public AssignmentApiService(
        HttpClient httpClient,
        AuthService authService)
   {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<StudentAssignmentModel>> GetMineAsync()
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            "api/assignment/me");

        if (request == null)
       {
            return new List<StudentAssignmentModel>();
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return new List<StudentAssignmentModel>();
        }

        return await response.Content
            .ReadFromJsonAsync<List<StudentAssignmentModel>>()
            ?? new List<StudentAssignmentModel>();
    }

    public async Task<StudentAssignmentModel?> SubmitAsync(
        Guid assignmentId,
        SubmitAssignmentModel model)
   {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            $"api/assignment/{assignmentId}/submit");

        if (request == null)
       {
            return null;
        }

        request.Content = JsonContent.Create(model);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
       {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<StudentAssignmentModel>();
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
