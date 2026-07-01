using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AxiCore.Diagnostics;
using AxiPlus.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AxiPlus.API.Endpoints;
using AxiPlus.API.Services;
using AxiCore.Security;
using AxiCore.Persistence;
using Microsoft.OpenApi.Models;
using AxiPlus.Application.Interfaces;
using AxiPlus.Infrastructure.Services;
using AxiForge.Infrastructure.Data;
using Microsoft.AspNetCore.DataProtection;
using AxiPlus.API.Extensions;
using AxiPlus.API.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter(level => level >= LogLevel.Warning);
builder.Logging.AddAxiCoreEnvironmentLogging(
    builder.Environment.ContentRootPath,
    "AxiPlus.API");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var dataProtectionPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("AxiPlus.API");

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<BatchAllocationService>();
builder.Services.AddTracedScoped<IDashboardService, DashboardService>();
builder.Services.AddTracedScoped<IModuleService, ModuleService>();
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
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
       {        
            Title = "AxiPlus.API",
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

            In = ParameterLocation.Header,

            Description =
                "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
       {        
           {        
                new OpenApiSecurityScheme
               {        
                    Reference =
                        new OpenApiReference
                       {        
                            Type = ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AxiCoreDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AxiCoreDb")));

builder.Services.AddDbContext<AxiForgeDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AxiForgeDb")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FunctionTraceActionFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy =>
       {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(
                     "http://localhost:5089",
                     "http://localhost:5090",
                     "http://localhost:5092",
                     "https://localhost:7246");
        });
});

builder.Services.AddTracedScoped<ILessonService, LessonService>();

builder.Services.AddTracedScoped<ILessonLiveClassService, LessonLiveClassService>();

builder.Services.AddTracedScoped<IAssignmentService, AssignmentService>();

builder.Services.AddTracedScoped<IAttendanceService, AttendanceService>();

builder.Services.AddTracedScoped<IStudentPortalService, StudentPortalService>();

builder.Services.AddTracedScoped<IMentorPortalService, MentorPortalService>();

builder.Services.AddTracedScoped<IAdminPortalService, AdminPortalService>();

builder.Services.AddTracedScoped<IOperationsService, OperationsService>();
builder.Services.AddTracedScoped<IPracticeLaunchService, PracticeLaunchService>();
builder.Services.AddScoped<AxiCoreAccountProvisioningService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();

    var axiCoreDbContext = scope.ServiceProvider
        .GetRequiredService<AxiCoreDbContext>();

    await axiCoreDbContext.Database.EnsureCreatedAsync();

    var axiForgeDbContext = scope.ServiceProvider
        .GetRequiredService<AxiForgeDbContext>();

    await axiForgeDbContext.Database.EnsureCreatedAsync();

    if (axiForgeDbContext.Database.IsNpgsql())
    {
        await axiForgeDbContext.Database.ExecuteSqlRawAsync("""
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
app.UseCors("AllowBlazor");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUserEndpoints();
app.MapTrackEndpoints();
app.MapStudentEndpoints();
app.MapModuleEndpoints();

app.Run();

