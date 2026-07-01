using AxiPlus.Application.DTOs.Users;
using AxiPlus.API.Filters;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using AxiPlus.API.Services;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AxiCore.Identity;

namespace AxiPlus.API.Endpoints;

public static class UserEndpoints
{        
    public static void MapUserEndpoints(
        this WebApplication app)
   {
        var group = app.MapGroup("/api/users");
        group.AddEndpointFilter<FunctionTraceEndpointFilter>();

        // CREATE USER
        group.MapPost("/", async (
            CreateUserDto dto,
            AppDbContext context,
            AxiCoreAccountProvisioningService axiCoreProvisioningService,
            CancellationToken cancellationToken) =>
       {
            var exists = await context.Users
                .AnyAsync(x => x.Email == dto.Email);

            if (exists)
           {
                return Results.BadRequest(
                    "Email already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                dto.Password);

            var user = new User
           {       
                Id = Guid.NewGuid(),

                FullName = dto.FullName,

                Email = dto.Email,

                PasswordHash = passwordHash,

                RoleId = dto.Role,

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            await context.SaveChangesAsync(cancellationToken);

            var role = await context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.Role, cancellationToken);

            if (role?.Name == "Student")
            {
                await axiCoreProvisioningService.ProvisionStudentAsync(
                    dto.FullName,
                    dto.Email,
                    passwordHash,
                    new[] { AxiCoreProductCodes.AxiPlus, AxiCoreProductCodes.AxiForge },
                    cancellationToken);
            }

            return Results.Ok();
        });

        // GET USERS
        group.MapGet("/", async (
            AppDbContext context) =>
       {
            var users = await context.Users
                .Select(x => new UserDto
                {
                    
                    Id = x.Id,

                    FullName = x.FullName,

                    Email = x.Email,

                    Role = x.Role.Name,

                    IsActive = x.IsActive
                })
                .ToListAsync();

            return Results.Ok(users);
        });
    }
}
