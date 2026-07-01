using Microsoft.Extensions.Logging;

namespace AxiCore.Diagnostics;

/// <summary>
/// Provides consistent entering, exiting, and exception logging for functions across AxiCore products.
/// Returns an IDisposable trace scope so functions can log lifecycle events without duplicating trace strings everywhere.
/// </summary>
public sealed class FunctionTrace : IDisposable
{
    private readonly ILogger? _logger;
    private readonly string _fileName;
    private readonly string _functionName;
    private bool _disposed;

    private FunctionTrace(ILogger? logger, string fileName, string functionName)
    {
        _logger = logger;
        _fileName = fileName;
        _functionName = functionName;
        Write($"Entering -> {_fileName} -> {_functionName}");
    }

    public static FunctionTrace Enter(ILogger? logger, string fileName, string functionName)
    {
        Console.WriteLine("Entering -> FunctionTrace.cs -> Enter");
        try
        {
            return new FunctionTrace(logger, fileName, functionName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> FunctionTrace.cs -> Enter -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> FunctionTrace.cs -> Enter");
        }
    }

    public void Exception(Exception exception)
    {
        Console.WriteLine("Entering -> FunctionTrace.cs -> Exception");
        try
        {
            Write($"Exception -> {_fileName} -> {_functionName} -> {exception.Message}", exception);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> FunctionTrace.cs -> Exception -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> FunctionTrace.cs -> Exception");
        }
    }

    /// <summary>
    /// Writes a trace line before evaluating an if branch.
    /// Returns the condition so callers can keep normal if syntax while logging the branch decision.
    /// </summary>
    public static bool If(string productName, string folderName, string fileName, string functionName, string conditionName, bool condition)
    {
        Console.WriteLine("Entering -> FunctionTrace.cs -> If");
        try
        {
            Console.WriteLine($"If -> {productName} -> {folderName} -> {fileName} -> {functionName} -> {conditionName} -> {condition}");
            return condition;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> FunctionTrace.cs -> If -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> FunctionTrace.cs -> If");
        }
    }

    /// <summary>
    /// Writes a trace line when entering a loop block.
    /// Returns no value because loop tracing is informational.
    /// </summary>
    public static void Loop(string productName, string folderName, string fileName, string functionName, string loopName, int? index = null)
    {
        Console.WriteLine("Entering -> FunctionTrace.cs -> Loop");
        try
        {
            var suffix = index.HasValue ? $" -> Index:{index.Value}" : string.Empty;
            Console.WriteLine($"Loop -> {productName} -> {folderName} -> {fileName} -> {functionName} -> {loopName}{suffix}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> FunctionTrace.cs -> Loop -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> FunctionTrace.cs -> Loop");
        }
    }

    /// <summary>
    /// Writes a trace line before switch handling.
    /// Returns the switch value so callers can preserve existing switch expressions or statements.
    /// </summary>
    public static T Switch<T>(string productName, string folderName, string fileName, string functionName, string switchName, T value)
    {
        Console.WriteLine("Entering -> FunctionTrace.cs -> Switch");
        try
        {
            Console.WriteLine($"Switch -> {productName} -> {folderName} -> {fileName} -> {functionName} -> {switchName} -> {value}");
            return value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> FunctionTrace.cs -> Switch -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> FunctionTrace.cs -> Switch");
        }
    }

    /// <summary>
    /// Writes a trace line for a switch case or if/else branch body.
    /// Returns no value because it marks the selected branch.
    /// </summary>
    public static void Branch(string productName, string folderName, string fileName, string functionName, string branchName)
    {
        Console.WriteLine("Entering -> FunctionTrace.cs -> Branch");
        try
        {
            Console.WriteLine($"Branch -> {productName} -> {folderName} -> {fileName} -> {functionName} -> {branchName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> FunctionTrace.cs -> Branch -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> FunctionTrace.cs -> Branch");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Write($"Exiting -> {_fileName} -> {_functionName}");
        _disposed = true;
    }

    private void Write(string message, Exception? exception = null)
    {
        Console.WriteLine(message);
        if (exception is null)
        {
            return;
        }

        _logger?.LogError(exception, "{TraceMessage}", message);
    }
}
