using AxiHire.Application.Interfaces;
using AxiHire.Infrastructure.Data;
using AxiHire.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AxiHire.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAxiHireInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Console.WriteLine("Entering -> AxiHire.Infrastructure -> DependencyInjection.cs -> AddAxiHireInfrastructure");
        try
        {
            services.AddDbContext<AxiHireDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AxiHireDb")));

            services.AddScoped<ICandidateVerificationService, CandidateVerificationService>();

            return services;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiHire.Infrastructure -> DependencyInjection.cs -> AddAxiHireInfrastructure -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiHire.Infrastructure -> DependencyInjection.cs -> AddAxiHireInfrastructure");
        }
    }
}
