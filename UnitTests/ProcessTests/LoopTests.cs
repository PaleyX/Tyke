using System.Diagnostics;

namespace UnitTests.ProcessTests;

[Collection("Non-Parallel Collection")]
public class LoopTests
{
    [Theory]
    [InlineData("100", "a")]
    [InlineData("50", "b")]
    [InlineData("1", "c")]
    [InlineData("1000", "d")]
    public async Task Loop_Until_Exits_Correctly(string value, string result)
    {
        var path = Tools.WriteControlFile("LoopUntilEndloop", "[VALUE]", value);

        try
        {
            var startInfo = Tools.GetTykeConsole(path);

            using var process = Process.Start(startInfo)!;

            var stdout = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Assert.Equal(0, process.ExitCode);
            Assert.Contains($"Result: [{result}]", stdout);
            Assert.True(string.IsNullOrWhiteSpace(stderr), "StdErr not empty");
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
