using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceTests
{
    public class DataBufferIncrement
    {
        private readonly byte[] _buffer = new byte[1024];

        [Params(10, 30, 1000)]
        public int Offset;

        [Benchmark(Baseline = true)]
        public void BlockCopy()
        {
            var value = BitConverter.ToUInt32(_buffer, Offset);
            value++;
            var bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, _buffer, Offset, 4);
        }

        [Benchmark]
        public void Unsafe1()
        {
            unsafe
            {
                fixed (byte* p = _buffer)
                {
                    var value = (uint*)(p + Offset);
                    ++*value;
                }
            }
        }

        [Benchmark]
        public void Unsafe2()
        {
            unsafe
            {
                fixed (byte* p = _buffer)
                {
                    (*(uint*)(p + Offset))++;
                }
            }
        }

        [Benchmark]
        public void Safe()
        {
            var span = _buffer.AsSpan(Offset, 4);
            var value = MemoryMarshal.Cast<byte, uint>(span)[0];
            value++;
            MemoryMarshal.Cast<uint, byte>(MemoryMarshal.CreateSpan(ref value, 1)).CopyTo(span);
        }
    }
}
