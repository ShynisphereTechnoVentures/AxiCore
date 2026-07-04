using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AxiCore.Diagnostics;

namespace AxiPlus.Web.Services;

public sealed class AuthorizedApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ILogger<AuthorizedApiClient> _logger;

    public AuthorizedApiClient(
        HttpClient httpClient,
        AuthService authService,
        ILogger<AuthorizedApiClient> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Sends an authorized GET request and deserializes the response.
    /// Returns default when the user is unauthenticated or the API request fails.
    /// </summary>
    public async Task<T?> GetAsync<T>(string url)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthorizedApiClient), nameof(GetAsync));
        try
        {
            var request = await CreateRequestAsync(HttpMethod.Get, url);
            return request == null ? default : await SendAsync<T>(request);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return default;
        }
    }

    /// <summary>
    /// Sends an authorized GET request and returns an empty list on failure.
    /// Returns a list so pages can render stable empty states.
    /// </summary>
    public async Task<List<T>> GetListAsync<T>(string url)
    {
        var result = await GetAsync<List<T>>(url);
        return result ?? new List<T>();
    }

    /// <summary>
    /// Sends an authorized POST request and deserializes the response.
    /// Returns default when the user is unauthenticated or the API request fails.
    /// </summary>
    public async Task<T?> PostAsync<T>(string url, object body)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthorizedApiClient), nameof(PostAsync));
        try
        {
            var request = await CreateRequestAsync(HttpMethod.Post, url);
            if (request == null)
            {
                return default;
            }

            request.Content = JsonContent.Create(body);
            return await SendAsync<T>(request);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            return default;
        }
    }

    /// <summary>
    /// Sends an authorized POST request and returns a list response.
    /// Returns an empty list when the request cannot be completed.
    /// </summary>
    public async Task<List<T>> PostListAsync<T>(string url, object body)
    {
        var result = await PostAsync<List<T>>(url, body);
        return result ?? new List<T>();
    }

    /// <summary>
    /// Sends an authorized request without reading a response body.
    /// Returns true when the API responds with a success status code.
    /// </summary>
    public async Task<bool> SendAsync(HttpMethod method, string url, object? body = null)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthorizedApiClient), nameof(SendAsync));
        try
        {
            var request = await CreateRequestAsync(method, url);
            if (request == null)
            {
                return false;
            }

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            trace.Exception(ex);
            return false;
        }
    }

    private async Task<T?> SendAsync<T>(HttpRequestMessage request)
    {
        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "AxiPlus API request failed with status {StatusCode}",
                    response.StatusCode);
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or NotSupportedException or JsonException)
        {
            _logger.LogWarning(ex, "AxiPlus API response could not be read.");
            return default;
        }
    }

    private async Task<HttpRequestMessage?> CreateRequestAsync(HttpMethod method, string url)
    {
        var token = await _authService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
