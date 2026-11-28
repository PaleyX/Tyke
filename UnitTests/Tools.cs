using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace UnitTests;

internal static class Tools
{
    internal static string WriteControlFile(string name, string search, string replace)
    {
        var a = Assembly.GetExecutingAssembly();

        using var stream = a.GetManifestResourceStream($"UnitTests.Programs.{name}.txt");
        if (stream is null)
        {
            throw new Exception($"Cannot find resource: {name}");
        }

        string content;

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            content = reader.ReadToEnd();
        }

        content = content.Replace(search, replace);

        var pathname = Path.GetTempFileName();
        File.WriteAllText(pathname, content);

        return pathname;
    }

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
