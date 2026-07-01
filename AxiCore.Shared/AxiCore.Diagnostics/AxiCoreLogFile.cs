namespace AxiCore.Diagnostics;

internal static class AxiCoreLogFile
{
    private const string MutexName = "Global\\AxiCoreEnvironmentLogFileMutex";

    public static void AppendLine(string filePath, string line)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var mutex = new Mutex(false, MutexName);
        var hasHandle = false;

        try
        {
            hasHandle = mutex.WaitOne(TimeSpan.FromSeconds(10));
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
        finally
        {
            if (hasHandle)
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
