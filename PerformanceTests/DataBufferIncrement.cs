using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PerformanceTests;

public class DataBufferIncrement
{
    private readonly byte[] _buffer = new byte[1024];

    //[Params(10, 31, 1019)]
    //public int Offset;

    [Benchmark(Baseline = true)]
    public void BlockCopy()
    {
        for (int i = 0; i < 1000; ++i)
        {
            var value = BitConverter.ToUInt32(_buffer, i);
            value++;
            var bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, _buffer, i, 4);
        }
    }

    [Benchmark]
    public void Unsafe1()
    {
        for (int i = 0; i < 1000; ++i)
        {
            unsafe
            {
                fixed (byte* p = _buffer)
                {
                    var value = (uint*)(p + i);
                    ++*value;
                }
            }
        }
    }

    [Benchmark]
    public void Unsafe2()
    {
        for (int i = 0; i < 1000; ++i)
        {
            unsafe
            {
                fixed (byte* p = _buffer)
                {
                    (*(uint*)(p + i))++;
                }
            }
        }
    }

    [Benchmark]
    public void Safe()
    {
        for (int i = 0; i < 1000; ++i)
        {
            var span = _buffer.AsSpan(i, 4);
            var value = MemoryMarshal.Cast<byte, uint>(span)[0];
            value++;
            MemoryMarshal.Cast<uint, byte>(MemoryMarshal.CreateSpan(ref value, 1)).CopyTo(span);
        }
    }

    [Benchmark]
    public void Safe2()
    {
        for (int i = 0; i < 1000; ++i)
        {
            ref uint value = ref Unsafe.As<byte, uint>(ref _buffer[i]);
            value++;
        }
    }
}