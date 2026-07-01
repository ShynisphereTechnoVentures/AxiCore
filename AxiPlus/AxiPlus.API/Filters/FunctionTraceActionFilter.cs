using AxiCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AxiPlus.API.Filters;

/// <summary>
/// Traces every MVC controller action in AxiPlus.
/// Returns action execution unchanged while providing entering, exiting, and exception logging for API action functions.
/// </summary>
public sealed class FunctionTraceActionFilter : IAsyncActionFilter
{
    private readonly ILogger<FunctionTraceActionFilter> _logger;

    public FunctionTraceActionFilter(ILogger<FunctionTraceActionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Runs around each controller action to log function lifecycle events.
    /// Returns the original action result so API behavior remains unchanged.
    /// </summary>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var descriptor = context.ActionDescriptor;
        var fileName = descriptor.RouteValues.TryGetValue("controller", out var controller)
            ? $"{controller}Controller.cs"
            : "Controller.cs";
        var functionName = descriptor.RouteValues.TryGetValue("action", out var action)
            ? action ?? "Unknown"
            : "Unknown";

        using var trace = FunctionTrace.Enter(_logger, fileName, functionName);
        try
        {
            var executed = await next();
            if (executed.Exception is not null && !executed.ExceptionHandled)
            {
                trace.Exception(executed.Exception);
            }
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
