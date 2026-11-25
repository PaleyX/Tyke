using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace UnitTests.ProcessTests;

[Collection("Non-Parallel Collection")]
public class SwitchTests
{
    [Theory]
    [InlineData("0", "Default")]
    [InlineData("1", "One")]
    [InlineData("2", "Two")]
    [InlineData("3", "Three")]
    public async Task Switch_Simple_Branches_Correctly(string value, string result)
    {
        var path = WriteControlFile("SwitchTestSimple", "[VALUE]", value);

        try
        {
            var startInfo = Tools.GetTykeConsole(path);
            
            using var process = Process.Start(startInfo)!;

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

    [Theory]
    [InlineData("0", "Default")]
    [InlineData("3", "A")]
    [InlineData("9", "B")]
    [InlineData("12", "C")]
    public async Task Switch_Expressions_Branches_Correctly(string value, string result)
    {
        var path = WriteControlFile("SwitchTestExpressions", "[VALUE]", value);

        try
        {
            var startInfo = Tools.GetTykeConsole(path);

            using var process = Process.Start(startInfo)!;

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

