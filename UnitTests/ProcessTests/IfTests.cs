using System.Diagnostics;
using System.Text.RegularExpressions;

namespace UnitTests.ProcessTests;

[Collection("Non-Parallel Collection")]
public class IfTests
{
    [Theory]
    [InlineData("2", "b")]
    [InlineData("1", "c")]
    [InlineData("3", "b")]
    [InlineData("100", "d")]
    [InlineData("49", "x")]
    public async Task If_Else_Endif_Works_Correctly(string value, string result)
    {
        var path = Tools.WriteControlFile("IfElseEndif", "[VALUE]", value);

        try
        {
            var startInfo = Tools.GetTykeConsole(path);

            using var process = Process.Start(startInfo)!;

            var stdout = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Assert.Equal(0, process.ExitCode);
            Assert.True(string.IsNullOrWhiteSpace(stderr), "StdErr not empty");

            var match = Regex.Match(stdout, @"Result:\s*\[[^,\]]+\]"); Assert.True(match.Success);
            Assert.True(match.Success);
            if (match.Success)
            {
                Assert.Equal($"Result: [{result}]", match.Value);
            }
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}