using System.Diagnostics;
using System.Text;

namespace AxiCore.Diagnostics;

public sealed class AxiCoreConsoleWriter : TextWriter
{
    private readonly TextWriter _inner;
    private readonly string _productName;
    private readonly string _contentRootPath;
    private readonly string _logFilePath;
    private readonly string _workspaceRootPath;

    public AxiCoreConsoleWriter(
        TextWriter inner,
        string productName,
        string contentRootPath,
        string logFilePath)
    {
        _inner = inner;
        _productName = productName;
        _contentRootPath = contentRootPath;
        _logFilePath = logFilePath;
        _workspaceRootPath = FindWorkspaceRoot(contentRootPath);
        var directory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public override Encoding Encoding => _inner.Encoding;

    public override void WriteLine(string? value)
    {
        var message = value ?? string.Empty;
        _inner.WriteLine(message);
        AxiCoreLogFile.AppendLine(_logFilePath, FormatConsoleLine(message));
    }

    public override void Write(char value)
    {
        _inner.Write(value);
    }

    private string FormatConsoleLine(string message)
    {
        var location = ResolveCallerLocation();
        var normalizedMessage = NormalizeFunctionTraceMessage(message, location);

        return
            $"{DateTimeOffset.Now:O} [Console] {_productName} {normalizedMessage}";
    }

    private string NormalizeFunctionTraceMessage(
        string message,
        CallerLocation location)
    {
        foreach (var action in new[] { "Entering", "Entered", "Exiting", "Exception" })
        {
            if (!message.StartsWith(action, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var functionName = location.FunctionName;
            var fileName = location.FileName;
            var folderName = location.FolderName;
            var parts = message.Split("->", StringSplitOptions.TrimEntries);
            if (parts.Length >= 3)
            {
                fileName = parts[1];
                functionName = parts[2];
            }

            if (folderName == "Unknown" ||
                fileName != location.FileName)
            {
                folderName = ResolveFolderFromFileName(fileName);
            }

            return
                $"{action} -> {_productName} -> {folderName} -> {fileName} -> {functionName}";
        }

        return
            $"Console -> {_productName} -> {location.FolderName} -> {location.FileName} -> {location.FunctionName} -> {message}";
    }

    private CallerLocation ResolveCallerLocation()
    {
        var stackTrace = new StackTrace(true);

        foreach (var frame in stackTrace.GetFrames() ?? Array.Empty<StackFrame>())
        {
            var method = frame.GetMethod();
            var declaringType = method?.DeclaringType;
            var typeName = declaringType?.FullName ?? string.Empty;

            if (string.IsNullOrWhiteSpace(typeName) ||
                typeName.StartsWith("System.", StringComparison.Ordinal) ||
                typeName.StartsWith("Microsoft.", StringComparison.Ordinal) ||
                typeName.StartsWith("AxiCore.Diagnostics.AxiCoreConsoleWriter", StringComparison.Ordinal))
            {
                continue;
            }

            var filePath = frame.GetFileName();
            var fileName = string.IsNullOrWhiteSpace(filePath)
                ? $"{declaringType?.Name ?? "Unknown"}.cs"
                : Path.GetFileName(filePath);

            var folderName = GetFolderName(filePath);
            return new CallerLocation(folderName, fileName, method?.Name ?? "Unknown");
        }

        return new CallerLocation("Unknown", "Unknown", "Unknown");
    }

    private string GetFolderName(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return "Unknown";
        }

        var folderPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            return "Unknown";
        }

        var relativePath = Path.GetRelativePath(_contentRootPath, folderPath);
        return relativePath.StartsWith("..", StringComparison.Ordinal)
            ? Path.GetFileName(folderPath)
            : relativePath;
    }

    private string ResolveFolderFromFileName(string fileName)
    {
        try
        {
            var match = Directory
                .EnumerateFiles(_workspaceRootPath, fileName, SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(path => path.Contains(_productName.Split('.')[0], StringComparison.OrdinalIgnoreCase))
                ?? Directory
                    .EnumerateFiles(_workspaceRootPath, fileName, SearchOption.AllDirectories)
                    .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                    .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(match))
            {
                return "Unknown";
            }

            var folderPath = Path.GetDirectoryName(match);
            return string.IsNullOrWhiteSpace(folderPath)
                ? "Unknown"
                : Path.GetRelativePath(_workspaceRootPath, folderPath);
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string FindWorkspaceRoot(string contentRootPath)
    {
        var directory = new DirectoryInfo(contentRootPath);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AxiCore.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return contentRootPath;
    }

    private sealed record CallerLocation(
        string FolderName,
        string FileName,
        string FunctionName);
}
