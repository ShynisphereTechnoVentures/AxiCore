using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AxiCore.Diagnostics;
using AxiPlus.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AxiPlus.Application.Services;

public class JwtService
{       
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
   {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Generates a JWT for an authenticated AxiPlus user.
    /// Returns a signed token containing identity and role claims so API and portal requests can authorize the user.
    /// </summary>
    public string GenerateToken(User user)
   {
        using var trace = FunctionTrace.Enter(_logger, nameof(JwtService), nameof(GenerateToken));
        try
        {
            var claims = new[]
           {   
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,user.Role?.Name ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
