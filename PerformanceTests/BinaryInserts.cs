using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PerformanceTests;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class BinaryInserts
{
    private byte[] _buffer = null!;
    private Consumer _consumer = null!;

    [GlobalSetup]
    public void Setup()
    {
        _buffer = new byte[1024];
        _consumer = new Consumer();
    }

    [Params((ushort)10, (ushort)255, (ushort)1000)]
    public ushort Value;

    [Params(0, 300, 600)]
    public int Offset;

    /// <summary>
    /// BitConverter + BlockCopy
    /// </summary>
    [Benchmark(Baseline = true)]
    public void Insert1()
    {
        var bytes = BitConverter.GetBytes(Value);
        Buffer.BlockCopy(bytes, 0, _buffer, Offset, 2);

        _consumer.Consume(
            BinaryPrimitives.ReadUInt16LittleEndian(
                _buffer.AsSpan(Offset)));
    }

    /// <summary>
    /// BinaryPrimitives
    /// </summary>
    [Benchmark]
    public void Insert2()
    {
        BinaryPrimitives.WriteUInt16LittleEndian(
            _buffer.AsSpan(Offset),
            Value);

        _consumer.Consume(
            BinaryPrimitives.ReadUInt16LittleEndian(
                _buffer.AsSpan(Offset)));
    }

    /// <summary>
    /// Pointer write
    /// </summary>
    [Benchmark]
    public unsafe void Insert3()
    {
        fixed (byte* p = _buffer)
        {
            *(ushort*)(p + Offset) = Value;

            _consumer.Consume(
                *(ushort*)(p + Offset));
        }
    }

    /// <summary>
    /// MemoryMarshal.Write
    /// </summary>
    [Benchmark]
    public void Insert4()
    {
        ushort value = Value;

        MemoryMarshal.Write(
            _buffer.AsSpan(Offset),
            in value);

        _consumer.Consume(
            MemoryMarshal.Read<ushort>(
                _buffer.AsSpan(Offset)));
    }

    /// <summary>
    /// Unsafe.WriteUnaligned
    /// </summary>
    [Benchmark]
    public void Insert5()
    {
        Unsafe.WriteUnaligned(
            ref _buffer[Offset],
            Value);

        _consumer.Consume(
            Unsafe.ReadUnaligned<ushort>(
                ref _buffer[Offset]));
    }
}