using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AxiCore.Diagnostics;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly LogLevel _minimumLevel;

    public FileLoggerProvider(string filePath, LogLevel minimumLevel = LogLevel.Warning)
    {
        _filePath = filePath;
        _minimumLevel = minimumLevel;
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _filePath, _minimumLevel);
    }

    public void Dispose()
    {
    }

    private sealed class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _filePath;
        private readonly LogLevel _minimumLevel;

        public FileLogger(string categoryName, string filePath, LogLevel minimumLevel)
        {
            _categoryName = categoryName;
            _filePath = filePath;
            _minimumLevel = minimumLevel;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minimumLevel;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var line =
                $"{DateTimeOffset.Now:O} [{logLevel}] {_categoryName} {message}";

            if (exception != null)
            {
                line += Environment.NewLine + exception;
            }

            AxiCoreLogFile.AppendLine(_filePath, line);
        }
    }
}

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddAxiCoreFileLogger(
        this ILoggingBuilder builder,
        string filePath,
        LogLevel minimumLevel = LogLevel.Warning)
    {
        builder.Services.AddSingleton<ILoggerProvider>(
            new FileLoggerProvider(filePath, minimumLevel));
        return builder;
    }
}
