
using System.Diagnostics;
using Repl;

while (true)
{
    Console.Write("> ");
    var command = Console.ReadLine()?.Trim() ?? "";

    // doing this so i can test operand/datfield comparisons 
    // copilot thinks they are the wrong way round

    if (command.StartsWith("&"))
    {
        var program = command[1..];

        var path = Tools.WriteControlFile(program);

        try
        {

            var startInfo = Tools.GetTykeConsole(path);
            using var process = Process.Start(startInfo);
            await process.WaitForExitAsync();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    else
    {
        Console.WriteLine("Unknown command");
    }
}
