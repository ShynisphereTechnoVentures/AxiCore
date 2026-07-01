using System.Reflection;
using Microsoft.Extensions.Logging;

namespace AxiCore.Diagnostics;

/// <summary>
/// Wraps interface-based services with function lifecycle tracing.
/// Returns a proxied service instance so existing AxiPlus services get entering, exiting, and exception logs without duplicating code in every method.
/// </summary>
public class TracingProxy<TService> : DispatchProxy
    where TService : class
{
    private TService _target = null!;
    private ILogger _logger = null!;
    private string _fileName = typeof(TService).Name;

    public static TService Create(TService target, ILogger logger, string? fileName = null)
    {
        Console.WriteLine("Entering -> TracingProxy.cs -> Create");
        try
        {
            var proxy = DispatchProxy.Create<TService, TracingProxy<TService>>();
            var tracingProxy = (TracingProxy<TService>)(object)proxy!;
            tracingProxy._target = target;
            tracingProxy._logger = logger;
            tracingProxy._fileName = fileName ?? target.GetType().Name;
            return proxy!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> TracingProxy.cs -> Create -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> TracingProxy.cs -> Create");
        }
    }

    /// <summary>
    /// Invokes the target service method while tracing entry, exit, and exceptions.
    /// Returns the target function result so calling code behaves the same as before the proxy was added.
    /// </summary>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var functionName = targetMethod?.Name ?? "Unknown";
        var trace = FunctionTrace.Enter(_logger, _fileName, functionName);

        try
        {
            var result = targetMethod!.Invoke(_target, args);

            if (result is Task task)
            {
                return WrapTask(task, trace, targetMethod.ReturnType);
            }

            trace.Dispose();
            return result;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            trace.Exception(ex.InnerException);
            trace.Dispose();
            throw ex.InnerException;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            trace.Dispose();
            throw;
        }
    }

    private static object WrapTask(Task task, FunctionTrace trace, Type returnType)
    {
        Console.WriteLine("Entering -> TracingProxy.cs -> WrapTask");
        try
        {
            if (returnType == typeof(Task))
            {
                return AwaitTask(task, trace);
            }

            var resultType = returnType.GetGenericArguments()[0];
            var method = typeof(TracingProxy<TService>)
                .GetMethod(nameof(AwaitGenericTask), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(resultType);

            return method.Invoke(null, new object[] { task, trace })!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> TracingProxy.cs -> WrapTask -> {ex.Message}");
            trace.Exception(ex);
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> TracingProxy.cs -> WrapTask");
        }
    }

    private static async Task AwaitTask(Task task, FunctionTrace trace)
    {
        Console.WriteLine("Entering -> TracingProxy.cs -> AwaitTask");
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
        finally
        {
            trace.Dispose();
            Console.WriteLine("Exiting -> TracingProxy.cs -> AwaitTask");
        }
    }

    private static async Task<TResult> AwaitGenericTask<TResult>(Task task, FunctionTrace trace)
    {
        Console.WriteLine("Entering -> TracingProxy.cs -> AwaitGenericTask");
        try
        {
            return await (Task<TResult>)task;
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
        finally
        {
            trace.Dispose();
            Console.WriteLine("Exiting -> TracingProxy.cs -> AwaitGenericTask");
        }
    }
}
