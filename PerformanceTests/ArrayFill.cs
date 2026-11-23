using BenchmarkDotNet.Attributes;
using Tyke.Net.Data;

namespace PerformanceTests
{
    public class ArrayFill
    {
        private DataBuffer _buffer;

        [Params(10, 30, 1000)]
        public int Length;

        [Params(32, 16, 128)]
        public byte Value;

        [GlobalSetup]
        public void Setup()
        {
            _buffer = new DataBuffer(Length);
        }

        [Benchmark(Baseline = true)]
        public void FillArea()
        {
            _buffer.FillArea(Value);
        }

        [Benchmark]
        public void FillAreaX()
        {
            //_buffer.FillAreaX(Value);
        }
    }
}
