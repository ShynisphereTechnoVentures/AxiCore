using AxiForge.Application.Interfaces;
using AxiForge.Infrastructure.Data;
using AxiForge.Infrastructure.Options;
using AxiForge.Infrastructure.Services;
using AxiCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AxiForge.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers AxiForge infrastructure dependencies.
    /// Returns the service collection so API and worker hosts can share the same wiring.
    /// </summary>
    public static IServiceCollection AddAxiForgeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Console.WriteLine("Entering -> DependencyInjection.cs -> AddAxiForgeInfrastructure");
        try
        {
            services.AddDbContext<AxiForgeDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AxiForgeDb")));

            services.AddDbContext<AxiCoreDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AxiCoreDb")));

            services.AddSingleton(Microsoft.Extensions.Options.Options.Create(CreateJudge0Options(configuration)));
            services.AddSingleton(Microsoft.Extensions.Options.Options.Create(CreateEmailDeliveryOptions(configuration)));
            services.AddScoped<IAuthService, AxiForgeAuthService>();
            services.AddScoped<IEmailDeliveryService, EmailDeliveryService>();
            services.AddScoped<IStudentDashboardService, StudentDashboardService>();
            services.AddScoped<ILaunchTokenService, LaunchTokenService>();
            services.AddScoped<IJudge0Client, Judge0Client>();
            services.AddScoped<ICodingPracticeService, CodingPracticeService>();
            services.AddScoped<IRoadmapService, RoadmapService>();
            services.AddScoped<IAssessmentService, AssessmentService>();

            return services;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> DependencyInjection.cs -> AddAxiForgeInfrastructure -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> DependencyInjection.cs -> AddAxiForgeInfrastructure");
        }
    }

    private static Judge0Options CreateJudge0Options(IConfiguration configuration)
    {
        var section = configuration.GetSection("Judge0");
        var options = new Judge0Options
        {
            Enabled = bool.TryParse(section["Enabled"], out var enabled) && enabled,
            BaseUrl = section["BaseUrl"] ?? string.Empty,
            ApiKey = section["ApiKey"] ?? string.Empty,
            ApiKeyHeaderName = section["ApiKeyHeaderName"] ?? "X-RapidAPI-Key",
            PollIntervalMilliseconds = int.TryParse(section["PollIntervalMilliseconds"], out var pollInterval)
                ? pollInterval
                : 750,
            MaxPollAttempts = int.TryParse(section["MaxPollAttempts"], out var maxPollAttempts)
                ? maxPollAttempts
                : 20
        };

        var languageSection = section.GetSection("LanguageIds");
        foreach (var language in languageSection.GetChildren())
        {
            if (!string.IsNullOrWhiteSpace(language.Key) &&
                int.TryParse(language.Value, out var languageId))
            {
                options.LanguageIds[language.Key] = languageId;
            }
        }

        return options;
    }

    private static EmailDeliveryOptions CreateEmailDeliveryOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection("EmailDelivery");
        var smtpSection = section.GetSection("Smtp");

        return new EmailDeliveryOptions
        {
            Mode = section["Mode"] ?? "Console",
            FromEmail = section["FromEmail"] ?? "no-reply@axionora.local",
            AppBaseUrl = section["AppBaseUrl"] ?? "http://localhost:5242",
            Smtp = new SmtpDeliveryOptions
            {
                Host = smtpSection["Host"] ?? string.Empty,
                Port = int.TryParse(smtpSection["Port"], out var port) ? port : 587,
                Username = smtpSection["Username"] ?? string.Empty,
                Password = smtpSection["Password"] ?? string.Empty,
                EnableSsl = !bool.TryParse(smtpSection["EnableSsl"], out var enableSsl) || enableSsl
            }
        };
    }
}
