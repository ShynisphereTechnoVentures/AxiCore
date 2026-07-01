using AxiForge.Application.DTOs.Launch;

namespace AxiForge.Application.Interfaces;

public interface ILaunchTokenService
{
    LaunchValidationResponseDto Validate(string token);
}
