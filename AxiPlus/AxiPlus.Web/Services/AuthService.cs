using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

using AxiCore.Diagnostics;
using AxiPlus.Web.Models;

namespace AxiPlus.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        HttpClient httpClient,
        IJSRuntime jsRuntime,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;

        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user against the AxiPlus API.
    /// Returns login details when authentication succeeds so the portal can store the JWT and role context.
    /// </summary>
    public async Task<LoginResponseDto?> LoginAsync(
     string email,
     string password)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(LoginAsync));
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/login",
                new LoginRequestDto
                {
                    Email = email,
                    Password = password
                });

            if (!response.IsSuccessStatusCode)
                return null;

            var result =
                await response.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            if (result == null)
                return null;

            await SaveAuthDataAsync(
                result.Token,
                result.Role,
                result.FullName);

            return result;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);

            return null;
        }
    }
    /// <summary>
    /// Saves the JWT token in browser storage.
    /// Returns no value because it updates client-side authentication state.
    /// </summary>
    public async Task SaveTokenAsync(string token)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(SaveTokenAsync));
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "auth.setToken",
                token);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Reads the JWT token from browser storage.
    /// Returns the token when available so API client services can authorize requests.
    /// </summary>
    public async Task<string?> GetTokenAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(GetTokenAsync));
        try
        {
            Console.WriteLine("GetTokenAsync");
            return await _jsRuntime.InvokeAsync<string>(
                "auth.getToken");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : " + ex);
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Clears authentication data from browser storage.
    /// Returns no value because it signs the current portal user out locally.
    /// </summary>
    public async Task LogoutAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(LogoutAsync));
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "auth.logout");
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// Saves token, role, and display name in browser storage.
    /// Returns no value because it persists authentication context for later portal calls.
    /// </summary>
    public async Task SaveAuthDataAsync(
    string token,
    string role,
    string fullName)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(SaveAuthDataAsync));
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "auth.setAuthData",
                token,
                role,
                fullName);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Reads the current user's role from the stored JWT.
    /// Returns the role claim so the portal can route users to the correct dashboard.
    /// </summary>
    public async Task<string?> GetRoleAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(GetRoleAsync));
        var token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);

            var roleClaim = jwt.Claims.FirstOrDefault(x =>
                x.Type ==
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");


            Console.WriteLine("roleClaim?.Value : " + roleClaim?.Value);

            return roleClaim?.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : " + ex);
            trace.Exception(ex);

            return null;
        }
    }

    /// <summary>
    /// Reads the current user's identifier from the stored JWT.
    /// Returns the user ID when the token contains a valid name identifier claim.
    /// </summary>
    public async Task<Guid?> GetUserIdAsync()
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthService), nameof(GetUserIdAsync));
        var token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);

            var userIdClaim = jwt.Claims.FirstOrDefault(x =>
                x.Type ==
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (Guid.TryParse(userIdClaim?.Value, out var userId))
                return userId;

            return null;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);

            return null;
        }
    }
}
