using AxiForge.Application.Interfaces;
using AxiForge.Infrastructure.Data;
using AxiForge.Infrastructure.Services;
using AxiCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddScoped<IAuthService, AxiForgeAuthService>();
            services.AddScoped<IStudentDashboardService, StudentDashboardService>();
            services.AddScoped<ILaunchTokenService, LaunchTokenService>();
            services.AddScoped<IJudge0Client, LocalJudge0Client>();
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
}
