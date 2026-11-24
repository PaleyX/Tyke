using System.Diagnostics;
using System.Reflection;
using System.Text;
using Xunit.Abstractions;

namespace UnitTests.ProcessTests;

[Collection("Non-Parallel Collection")]
public class SwitchTests
{
    private readonly ITestOutputHelper _output;

    public SwitchTests(ITestOutputHelper output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    [Theory]
    [InlineData("0", "Default")]
    [InlineData("1", "One")]
    [InlineData("2", "Two")]
    [InlineData("3", "Three")]
    public async Task Gort(string value, string result)
    {
        var path = WriteControlFile("SwitchTest", "[VALUE]", value);

        try
        {
            var exe = Path.Combine(
                        AppContext.BaseDirectory,
                        "Tyke.Net.exe");

            var startInfo = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = path,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            var stdout = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Assert.Equal(0, process.ExitCode);
            Assert.Contains($"Result: [{result}]", stdout);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }



    private static string WriteControlFile(string name, string search, string replace)
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
}

