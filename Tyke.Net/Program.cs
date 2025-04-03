using System;
using System.Linq;

namespace Tyke.Net;

class Program
{
    static int Main(string[] args)
    {
        // header
        Console.WriteLine("Tyke Control File Interpreter");
        Console.WriteLine("64 bit: {0}", Environment.Is64BitProcess.ToString());


        if (!args.Any())
        {
            Console.WriteLine("No control file specified");
            return 1;
        }

        int result = Tyke.Run(args[0]);

        Console.WriteLine("Finished...");
        //Console.ReadKey();

        return result;
    }
}