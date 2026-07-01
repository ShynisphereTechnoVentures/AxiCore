using AxiCore.Diagnostics;

namespace AxiPlus.API.Extensions;

public static class TracedServiceCollectionExtensions
{
    /// <summary>
    /// Registers an interface service with the shared AxiCore tracing proxy.
    /// Returns the service collection so AxiPlus can consistently trace service functions through dependency injection.
    /// </summary>
    public static IServiceCollection AddTracedScoped<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        Console.WriteLine("Entering -> TracedServiceCollectionExtensions.cs -> AddTracedScoped");
        try
        {
            services.AddScoped<TImplementation>();
            services.AddScoped<TInterface>(sp =>
            {
                var target = sp.GetRequiredService<TImplementation>();
                var logger = sp.GetRequiredService<ILogger<TImplementation>>();
                return TracingProxy<TInterface>.Create(target, logger, $"{typeof(TImplementation).Name}.cs");
            });

            return services;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> TracedServiceCollectionExtensions.cs -> AddTracedScoped -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> TracedServiceCollectionExtensions.cs -> AddTracedScoped");
        }
    }
}
