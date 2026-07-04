using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AxiCore.Diagnostics;
using AxiCore.Identity;
using AxiCore.Persistence;
using AxiForge.Application.DTOs.Auth;
using AxiForge.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AxiForge.Infrastructure.Services;

public sealed class AxiForgeAuthService : IAuthService
{
    private readonly AxiCoreDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILaunchTokenService _launchTokenService;
    private readonly IEmailDeliveryService _emailDeliveryService;
    private readonly ILogger<AxiForgeAuthService> _logger;

    public AxiForgeAuthService(
        AxiCoreDbContext context,
        IConfiguration configuration,
        ILaunchTokenService launchTokenService,
        IEmailDeliveryService emailDeliveryService,
        ILogger<AxiForgeAuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _launchTokenService = launchTokenService;
        _emailDeliveryService = emailDeliveryService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates an AxiForge user with separate product credentials.
    /// Returns a JWT response when credentials are valid so the AxiForge portal can authorize API calls.
    /// </summary>
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(LoginAsync));
        try
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var account = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, cancellationToken);

            if (account == null ||
                !BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
            {
                return null;
            }

            var hasAxiForgeAccess = await _context.UserProductAccess
                .AnyAsync(
                    x => x.UserId == account.Id &&
                        x.ProductCode == AxiCoreProductCodes.AxiForge &&
                        (x.Status == AxiCoreProductAccessStatuses.Active ||
                            x.Status == AxiCoreProductAccessStatuses.Trial),
                    cancellationToken);

            if (!hasAxiForgeAccess)
            {
                return null;
            }

            account.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return CreateResponse(account);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Registers a new AxiForge student account.
    /// Returns a JWT response so the student can enter the dashboard immediately after registration.
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(RegisterAsync));
        try
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existing = await _context.Users
                .AnyAsync(x => x.Email == email, cancellationToken);

            if (existing)
            {
                throw new InvalidOperationException("An AxiForge account already exists for this email.");
            }

            var account = new AxiCoreUser
            {
                FullName = request.FullName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _context.Users.Add(account);
            await EnsureRoleAsync(account.Id, AxiCoreRoleNames.Student, cancellationToken);
            EnsureProductAccess(account.Id, AxiCoreProductCodes.AxiForge);
            await _context.SaveChangesAsync(cancellationToken);
            await SendEmailConfirmationAsync(account, cancellationToken);

            return CreateResponse(account);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Authenticates an AxiPlus-launched student into AxiForge with a local linked account.
    /// Returns a JWT response so child accounts can continue directly from AxiPlus into coding practice.
    /// </summary>
    public async Task<AuthResponseDto?> LoginFromLaunchAsync(string token, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(LoginFromLaunchAsync));
        try
        {
            var launch = _launchTokenService.Validate(token);
            if (!launch.IsValid || string.IsNullOrWhiteSpace(launch.StudentId))
            {
                return null;
            }

            var email = $"axiplus.child.{launch.StudentId}@axionora.local".ToLowerInvariant();
            var account = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

            if (account == null)
            {
                account = new AxiCoreUser
                {
                    FullName = $"AxiPlus Student {launch.StudentId[..8]}",
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                _context.Users.Add(account);
            }
            else
            {
                account.LastLoginAt = DateTime.UtcNow;
            }

            await EnsureRoleAsync(account.Id, AxiCoreRoleNames.Student, cancellationToken);
            EnsureProductAccess(account.Id, AxiCoreProductCodes.AxiForge);
            await _context.SaveChangesAsync(cancellationToken);
            return CreateResponse(account);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Captures a password reset request for an AxiForge account.
    /// Returns true even when no account is found so the endpoint does not disclose registered emails.
    /// </summary>
    public async Task<bool> RequestPasswordResetAsync(PasswordResetRequestDto request, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(RequestPasswordResetAsync));
        try
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var account = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, cancellationToken);

            if (account != null)
            {
                var token = CreatePurposeToken(account, "password-reset", TimeSpan.FromMinutes(30));
                await _emailDeliveryService.SendPasswordResetAsync(
                    account.Email,
                    account.FullName,
                    BuildAppLink("/reset-password", token),
                    cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Confirms an AxiForge email address using a signed short-lived token.
    /// Returns true when the token is valid and the account is marked confirmed.
    /// </summary>
    public async Task<bool> ConfirmEmailAsync(
        ConfirmEmailRequestDto request,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(ConfirmEmailAsync));
        try
        {
            var userId = ValidatePurposeToken(request.Token, "email-confirmation");
            if (userId == null)
            {
                return false;
            }

            var account = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId.Value && x.IsActive, cancellationToken);

            if (account == null)
            {
                return false;
            }

            account.EmailConfirmed = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Resends the email-confirmation message for an active account.
    /// Returns true even when no account exists so registered addresses are not disclosed.
    /// </summary>
    public async Task<bool> ResendEmailConfirmationAsync(
        ResendEmailConfirmationRequestDto request,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(ResendEmailConfirmationAsync));
        try
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var account = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, cancellationToken);

            if (account != null && !account.EmailConfirmed)
            {
                await SendEmailConfirmationAsync(account, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates an authentication response for an account.
    /// Returns token and profile details because both API and web callers need role-aware user context.
    /// </summary>
    private AuthResponseDto CreateResponse(AxiCoreUser account)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(CreateResponse));
        try
        {
            return new AuthResponseDto
            {
                Token = CreateToken(account),
                Email = account.Email,
                FullName = account.FullName,
                Role = AxiCoreRoleNames.Student,
                EmailConfirmed = account.EmailConfirmed
            };
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a signed JWT for an AxiForge account.
    /// Returns a compact token so clients can authorize protected AxiForge endpoints.
    /// </summary>
    private string CreateToken(AxiCoreUser account)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(CreateToken));
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.FullName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, AxiCoreRoleNames.Student)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a short-lived signed token for account support flows.
    /// Returns a JWT-like token so email confirmation and password reset can be verified later.
    /// </summary>
    private string CreatePurposeToken(AxiCoreUser account, string purpose, TimeSpan lifetime)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(CreatePurposeToken));
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim("purpose", purpose)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(lifetime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Writes the email-confirmation message to the shared console stream.
    /// Returns no value because SMTP delivery is configured outside the current development environment.
    /// </summary>
    private async Task SendEmailConfirmationAsync(
        AxiCoreUser account,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AxiForgeAuthService), nameof(SendEmailConfirmationAsync));
        try
        {
            var token = CreatePurposeToken(account, "email-confirmation", TimeSpan.FromDays(2));
            await _emailDeliveryService.SendEmailConfirmationAsync(
                account.Email,
                account.FullName,
                BuildAppLink("/confirm-email", token),
                cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    private string BuildAppLink(string path, string token)
    {
        var baseUrl = _configuration["EmailDelivery:AppBaseUrl"] ?? "http://localhost:5242";
        return $"{baseUrl.TrimEnd('/')}{path}?token={Uri.EscapeDataString(token)}";
    }

    private Guid? ValidatePurposeToken(string token, string expectedPurpose)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = key
                },
                out _);

            var purpose = principal.FindFirst("purpose")?.Value;
            var userIdValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return purpose == expectedPurpose &&
                Guid.TryParse(userIdValue, out var userId)
                    ? userId
                    : null;
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return null;
        }
    }

    private async Task EnsureRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);

        if (role == null)
        {
            role = new AxiCoreRole
            {
                Name = roleName
            };

            _context.Roles.Add(role);
        }

        var exists = await _context.UserRoles
            .AnyAsync(x => x.UserId == userId && x.RoleId == role.Id, cancellationToken);

        if (!exists)
        {
            _context.UserRoles.Add(
                new AxiCoreUserRole
                {
                    UserId = userId,
                    RoleId = role.Id
                });
        }
    }

    private void EnsureProductAccess(Guid userId, string productCode)
    {
        var exists = _context.UserProductAccess
            .Any(x => x.UserId == userId && x.ProductCode == productCode);

        if (exists)
        {
            return;
        }

        _context.UserProductAccess.Add(
            new AxiCoreProductAccess
            {
                UserId = userId,
                ProductCode = productCode,
                Status = AxiCoreProductAccessStatuses.Active,
                GrantedAt = DateTime.UtcNow
            });
    }
}
