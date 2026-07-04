using AxiCore.Diagnostics;
using AxiCore.Persistence;
using AxiHire.Application.Interfaces;
using AxiHire.Infrastructure;
using AxiHire.Infrastructure.Data;
using AxiHire.Infrastructure.Seed;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter(level => level >= LogLevel.Warning);
builder.Logging.AddAxiCoreEnvironmentLogging(
    builder.Environment.ContentRootPath,
    "AxiHire.API");

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var dataProtectionPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("AxiHire.API");

builder.Services.AddDbContext<AxiCoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AxiCoreDb")));

builder.Services.AddAxiHireInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AxiHireWeb", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:5267",
                "https://localhost:7267",
                "http://localhost:5067");
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var coreContext = scope.ServiceProvider.GetRequiredService<AxiCoreDbContext>();
    var hireContext = scope.ServiceProvider.GetRequiredService<AxiHireDbContext>();

    await coreContext.Database.EnsureCreatedAsync();
    await hireContext.Database.EnsureCreatedAsync();
    await AxiHireSeedData.SeedAsync(hireContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AxiHireWeb");
app.UseHttpsRedirection();

app.MapGet("/api/health", () => Results.Ok(new
{
    product = "AxiHire",
    status = "Healthy",
    utc = DateTime.UtcNow
}));

app.MapGet("/api/candidates", async (
    ICandidateVerificationService candidateService,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await candidateService.GetCandidatesAsync(cancellationToken));
});

app.MapGet("/api/candidates/{candidateId:guid}", async (
    Guid candidateId,
    ICandidateVerificationService candidateService,
    CancellationToken cancellationToken) =>
{
    var candidate = await candidateService.GetCandidateAsync(candidateId, cancellationToken);
    return candidate == null ? Results.NotFound() : Results.Ok(candidate);
});

app.Run();
