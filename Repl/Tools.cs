using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Repl;

internal static class Tools
{
    internal static string WriteControlFile(string name)
    {
        var a = Assembly.GetExecutingAssembly();

        using var stream = a.GetManifestResourceStream($"Repl.Programs.{name}.txt");
        if (stream is null)
        {
            throw new Exception($"Cannot find resource: {name}");
        }

        string content;

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            content = reader.ReadToEnd();
        }

        var pathname = Path.GetTempFileName();
        File.WriteAllText(pathname, content);

        return pathname;
    }

    internal static ProcessStartInfo GetTykeConsole(string programPath)
    {
        var exe = Path.Combine(
            AppContext.BaseDirectory,
            "Tyke.Net.exe");

        exe = exe.Replace("Repl", "Tyke.Net");

        var startInfo = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = programPath,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        return startInfo;
    }
}