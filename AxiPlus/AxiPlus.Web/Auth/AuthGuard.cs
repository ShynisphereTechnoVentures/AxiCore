using AxiPlus.Web.Services;

namespace AxiPlus.Web.Auth;

public static class AuthGuard
{       
    public static async Task<bool> IsAuthenticated(
        AuthService authService)
   {     
        var token = await authService.GetTokenAsync();

        return !string.IsNullOrEmpty(token);
    }
}