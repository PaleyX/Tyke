using System.Diagnostics;

namespace UnitTests;

internal static class Tools
{
    internal static ProcessStartInfo GetTykeConsole(string programPath)
    {
        var exe = Path.Combine(
            AppContext.BaseDirectory,
            "Tyke.Net.exe");

        var startInfo = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = programPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return startInfo;
    }
}
