using AxiCore.Diagnostics;

namespace AxiPlus.API.Filters;

/// <summary>
/// Traces minimal API endpoint functions in AxiPlus.
/// Returns the original endpoint result while adding entering, exiting, and exception logging for endpoint delegates.
/// </summary>
public sealed class FunctionTraceEndpointFilter : IEndpointFilter
{
    private readonly ILogger<FunctionTraceEndpointFilter> _logger;

    public FunctionTraceEndpointFilter(ILogger<FunctionTraceEndpointFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Runs around each minimal API endpoint delegate.
    /// Returns the endpoint result so routing behavior remains unchanged while lifecycle logging is added.
    /// </summary>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var functionName = endpoint?.DisplayName ?? context.HttpContext.Request.Path.Value ?? "Endpoint";
        using var trace = FunctionTrace.Enter(_logger, "MinimalApiEndpoint.cs", functionName);

        try
        {
            return await next(context);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
