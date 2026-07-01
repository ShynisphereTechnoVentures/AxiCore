using System.Security.Claims;
using System.Text;
using AxiCore.Diagnostics;
using AxiCore.Persistence;
using AxiCore.Security;
using AxiForge.Application.DTOs.Assessments;
using AxiForge.Application.DTOs.Auth;
using AxiForge.Application.DTOs.Coding;
using AxiForge.Application.Interfaces;
using AxiForge.Infrastructure;
using AxiForge.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter(level => level >= LogLevel.Warning);
builder.Logging.AddAxiCoreEnvironmentLogging(
    builder.Environment.ContentRootPath,
    "AxiForge.API");

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var dataProtectionPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("AxiForge.API");

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AxiForge.API",
        Version = "v1"
    });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddAxiForgeInfrastructure(builder.Configuration);
builder.Services.Configure<SignedTokenOptions>(
    builder.Configuration.GetSection("SignedTokens"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AxiForgeWeb", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:5242",
                "https://localhost:7110",
                "http://localhost:5290",
                "http://localhost:5055");
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var coreContext = scope.ServiceProvider.GetRequiredService<AxiCoreDbContext>();
    var context = scope.ServiceProvider.GetRequiredService<AxiForgeDbContext>();
    await coreContext.Database.EnsureCreatedAsync();
    await context.Database.EnsureCreatedAsync();

    if (context.Database.IsNpgsql())
    {
        await context.Database.ExecuteSqlRawAsync("""
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "InputFormat" text NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "OutputFormat" text NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "Constraints" text NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "Examples" text NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "Explanation" text NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "Tags" character varying(500) NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "StarterCodeByLanguage" text NOT NULL DEFAULT '';
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "TimeLimitMilliseconds" integer NOT NULL DEFAULT 1000;
            ALTER TABLE "CodingProblems" ADD COLUMN IF NOT EXISTS "MemoryLimitKb" integer NOT NULL DEFAULT 262144;
            """);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AxiForgeWeb");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapGet("/api/health", () => Results.Ok(new
{
    product = "AxiForge",
    status = "Healthy",
    utc = DateTime.UtcNow
}));

app.MapPost("/api/auth/register", async (
    RegisterRequestDto request,
    IAuthService authService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.FullName) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new
        {
            message = "Full name, email, and password are required."
        });
    }

    try
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new
        {
            message = ex.Message
        });
    }
});

app.MapPost("/api/auth/login", async (
    LoginRequestDto request,
    IAuthService authService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new
        {
            message = "Email and password are required."
        });
    }

    var result = await authService.LoginAsync(request, cancellationToken);
    return result == null ? Results.Unauthorized() : Results.Ok(result);
});

app.MapPost("/api/auth/request-password-reset", async (
    PasswordResetRequestDto request,
    IAuthService authService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
        return Results.BadRequest(new
        {
            message = "Email is required."
        });
    }

    var accepted = await authService.RequestPasswordResetAsync(request, cancellationToken);
    return accepted ? Results.Accepted() : Results.BadRequest();
});

app.MapPost("/api/auth/launch-login", async (
    string token,
    IAuthService authService,
    CancellationToken cancellationToken) =>
{
    var result = await authService.LoginFromLaunchAsync(token, cancellationToken);
    return result == null ? Results.Unauthorized() : Results.Ok(result);
});

app.MapGet("/api/dashboard/student/me", async (
    ClaimsPrincipal user,
    IStudentDashboardService dashboardService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await dashboardService.GetDashboardAsync(accountId, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.MapGet("/api/practice/launch", (
    string token,
    ILaunchTokenService launchTokenService) =>
{
    var result = launchTokenService.Validate(token);
    return result.IsValid ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapGet("/api/coding/problems", async (
    string? topic,
    string? difficulty,
    string? search,
    ICodingPracticeService codingPracticeService,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await codingPracticeService.GetProblemsAsync(topic, difficulty, search, cancellationToken));
});

app.MapGet("/api/coding/problems/{problemId:guid}", async (
    Guid problemId,
    ICodingPracticeService codingPracticeService,
    CancellationToken cancellationToken) =>
{
    var problem = await codingPracticeService.GetProblemAsync(problemId, cancellationToken);
    return problem == null ? Results.NotFound() : Results.Ok(problem);
});

app.MapPost("/api/coding/submissions", async (
    CreateSubmissionRequestDto request,
    ClaimsPrincipal user,
    ICodingPracticeService codingPracticeService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await codingPracticeService.SubmitAsync(accountId, request, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.MapGet("/api/coding/submissions/me", async (
    ClaimsPrincipal user,
    ICodingPracticeService codingPracticeService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await codingPracticeService.GetMySubmissionsAsync(accountId, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.MapGet("/api/roadmaps", async (
    IRoadmapService roadmapService,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await roadmapService.GetTemplatesAsync(cancellationToken));
});

app.MapGet("/api/roadmaps/{roadmapId:guid}", async (
    Guid roadmapId,
    IRoadmapService roadmapService,
    CancellationToken cancellationToken) =>
{
    var roadmap = await roadmapService.GetTemplateAsync(roadmapId, cancellationToken);
    return roadmap == null ? Results.NotFound() : Results.Ok(roadmap);
});

app.MapPost("/api/roadmaps/{roadmapId:guid}/enroll", async (
    Guid roadmapId,
    ClaimsPrincipal user,
    IRoadmapService roadmapService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await roadmapService.EnrollAsync(accountId, roadmapId, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.MapGet("/api/roadmaps/me", async (
    ClaimsPrincipal user,
    IRoadmapService roadmapService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await roadmapService.GetMyRoadmapsAsync(accountId, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.MapGet("/api/assessments", async (
    IAssessmentService assessmentService,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await assessmentService.GetTemplatesAsync(cancellationToken));
});

app.MapGet("/api/assessments/{assessmentId:guid}", async (
    Guid assessmentId,
    IAssessmentService assessmentService,
    CancellationToken cancellationToken) =>
{
    var assessment = await assessmentService.GetTemplateAsync(assessmentId, cancellationToken);
    return assessment == null ? Results.NotFound() : Results.Ok(assessment);
});

app.MapPost("/api/assessments/submit", async (
    SubmitAssessmentRequestDto request,
    ClaimsPrincipal user,
    IAssessmentService assessmentService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await assessmentService.SubmitAsync(accountId, request, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.MapGet("/api/assessments/results/me", async (
    ClaimsPrincipal user,
    IAssessmentService assessmentService,
    CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(userIdValue, out var accountId)
        ? Results.Ok(await assessmentService.GetMyResultsAsync(accountId, cancellationToken))
        : Results.Unauthorized();
})
.RequireAuthorization(policy => policy.RequireRole("Student"));

app.Run();
