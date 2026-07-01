using System.Net.Http.Headers;
using AxiPlus.Web.Models;
using AxiPlus.Application.DTOs.Dashboard;

namespace AxiPlus.Web.Services;

public class StudentService
{
    private readonly HttpClient _httpClient;

    private readonly AuthService _authService;

    public StudentService(
        HttpClient httpClient,
        AuthService authService)
    {
        _httpClient = httpClient;

        _authService = authService;
    }

    public async Task<StudentProfileDto?>
        GetProfileAsync()
    {
        try
        {
            var token =
                await _authService.GetTokenAsync();

            Console.WriteLine($"token{token}");

            if (string.IsNullOrEmpty(token))
                return null;

            var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "api/students/me");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var response =
                await _httpClient.SendAsync(request);

            Console.WriteLine(
                response.StatusCode);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<StudentProfileDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return null;
        }
    }

    public async Task<StudentBillingModel?> UpdateBillingAsync(
        UpdateStudentBillingModel model)
    {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Put,
            "api/students/me/billing");

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
            .ReadFromJsonAsync<StudentBillingModel>();
    }

    public async Task<StudentPaymentModel?> StartUpiPaymentAsync(
        UpdateStudentBillingModel model)
    {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            "api/students/me/payments/upi");

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
            .ReadFromJsonAsync<StudentPaymentModel>();
    }

    public async Task<List<StudentPaymentModel>> GetPaymentsAsync()
    {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            "api/students/me/payments");

        if (request == null)
        {
            return new List<StudentPaymentModel>();
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new List<StudentPaymentModel>();
        }

        return await response.Content
            .ReadFromJsonAsync<List<StudentPaymentModel>>()
            ?? new List<StudentPaymentModel>();
    }

    public async Task<StudentPaymentModel?> GetUpcomingPaymentAsync()
    {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            "api/students/me/payments/upcoming");

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
            .ReadFromJsonAsync<StudentPaymentModel>();
    }

    public async Task<List<StudentPlanModel>> GetPlansAsync()
    {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Get,
            "api/students/me/plans");

        if (request == null)
        {
            return new List<StudentPlanModel>();
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new List<StudentPlanModel>();
        }

        return await response.Content
            .ReadFromJsonAsync<List<StudentPlanModel>>()
            ?? new List<StudentPlanModel>();
    }

    public async Task<StudentBillingModel?> SelectPlanAsync(string planCode)
    {
        var request = await CreateAuthorizedRequestAsync(
            HttpMethod.Post,
            $"api/students/me/plans/{planCode}");

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
            .ReadFromJsonAsync<StudentBillingModel>();
    }


    public async Task<StudentDashboardDto?>
    GetDashboardAsync()
    {

        Console.WriteLine("GetDashboardAsync");
        try
        {
            var token =
                await _authService.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "api/dashboard/student/me");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var response =
                await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {

                Console.WriteLine("GetDashboardAsync = code = " + response.IsSuccessStatusCode);
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<StudentDashboardDto>();
        }
        catch
        {
            return null;
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
