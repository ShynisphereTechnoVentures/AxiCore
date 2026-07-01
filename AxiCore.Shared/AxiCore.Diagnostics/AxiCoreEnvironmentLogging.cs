using Microsoft.Extensions.Logging;

namespace AxiCore.Diagnostics;

public static class AxiCoreEnvironmentLogging
{
    public const string DefaultLogFileName = "axicore-environment.log";

    public static ILoggingBuilder AddAxiCoreEnvironmentLogging(
        this ILoggingBuilder builder,
        string contentRootPath,
        string productName)
    {
        return builder;
    }

    public static string GetEnvironmentLogFilePath(string contentRootPath)
    {
        var root = FindWorkspaceRoot(contentRootPath);
        return Path.Combine(root, "Logs", DefaultLogFileName);
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
}
