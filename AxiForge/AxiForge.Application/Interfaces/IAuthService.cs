using AxiForge.Application.DTOs.Auth;

namespace AxiForge.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    Task<AuthResponseDto?> LoginFromLaunchAsync(string token, CancellationToken cancellationToken);

    Task<bool> RequestPasswordResetAsync(PasswordResetRequestDto request, CancellationToken cancellationToken);

    Task<bool> ConfirmEmailAsync(ConfirmEmailRequestDto request, CancellationToken cancellationToken);

    Task<bool> ResendEmailConfirmationAsync(ResendEmailConfirmationRequestDto request, CancellationToken cancellationToken);
}
