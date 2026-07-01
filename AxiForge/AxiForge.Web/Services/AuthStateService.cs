using AxiForge.Application.DTOs.Auth;

namespace AxiForge.Web.Services;

public sealed class AuthStateService
{
    public AuthResponseDto? CurrentUser { get; private set; }

    public string? Token => CurrentUser?.Token;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    /// <summary>
    /// Stores the current authenticated user for the active Blazor circuit.
    /// Returns no value because it updates local portal session state.
    /// </summary>
    public void SetUser(AuthResponseDto user)
    {
        Console.WriteLine("Entering -> AuthStateService.cs -> SetUser");
        try
        {
            CurrentUser = user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AuthStateService.cs -> SetUser -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AuthStateService.cs -> SetUser");
        }
    }

    /// <summary>
    /// Clears the active AxiForge user.
    /// Returns no value because it signs the current Blazor circuit out locally.
    /// </summary>
    public void Clear()
    {
        Console.WriteLine("Entering -> AuthStateService.cs -> Clear");
        try
        {
            CurrentUser = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AuthStateService.cs -> Clear -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AuthStateService.cs -> Clear");
        }
    }
}
