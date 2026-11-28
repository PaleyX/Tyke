using BenchmarkDotNet.Running;

namespace PerformanceTests;

internal class Program
{
    public static void Main(string[] args)
    {
        //var summary = BenchmarkRunner.Run<DataBufferIncrement>();
        //var summary = BenchmarkRunner.Run<ArrayFill>();
        var summary = BenchmarkRunner.Run<BinaryInserts>();
    }
}