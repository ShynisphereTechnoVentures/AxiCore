using AxiPlus.Application.DTOs.Auth;
using AxiPlus.Application.Services;
using AxiCore.Diagnostics;
using AxiPlus.Infrastructure.Data;
using AxiPlus.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthController> _logger;


    public AuthController(
        AppDbContext context,
        JwtService jwtService,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates an AxiPlus user with email and password.
    /// Returns a JWT login response when credentials, account status, and student billing access are valid.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(AuthController), nameof(Login));
        try
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                Console.WriteLine("Invalid email or password");
                return Unauthorized("Invalid email or password");
            }

            var validPassword = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

            if (!validPassword)
            {
                Console.WriteLine("InvalidPassword");
                return Unauthorized("Invalid email or password");
            }

            if (!user.IsActive)
            {
                Console.WriteLine("Account is inactive.");
                return Unauthorized("Account is inactive.");
            }

            if (user.Role.Name == "Student")
            {
                Console.WriteLine("valid Student");
                var billing = await _context.StudentBillingAccounts
                    .Include(x => x.Student)
                    .FirstOrDefaultAsync(x => x.Student.UserId == user.Id);

                if (billing != null &&
                    billing.Status != BillingStatus.Active &&
                    billing.Status != BillingStatus.Trial &&
                    DateTime.UtcNow > billing.GraceEndsAt)
                {
                    billing.Status = BillingStatus.Locked;
                    await _context.SaveChangesAsync();

                    return Unauthorized(
                        "Payment window expired. Please complete payment to continue.");
                }
            }

            var token = _jwtService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Role = user.Role.Name
            };

            Console.WriteLine("valid Student : " + user.FullName);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : " + ex);
            trace.Exception(ex);
            throw;
        }
    }
}
