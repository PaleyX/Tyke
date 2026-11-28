using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tyke.Net.Data;

internal class DataBuffer
{
    private readonly int _areaLength;
    private readonly byte[] _buffer;

    internal DataBuffer(int length)
    {
        Debug.Assert(length > 0);

        _areaLength = length;

        _buffer = new byte[length];
        CurrentBuffer = this;

        FillArea();
    }

    // most recently set buffer
    internal static DataBuffer CurrentBuffer { get; private set; }

    internal bool IsValidArea() => _areaLength > 0;

    internal int AreaLength => _areaLength;

    internal void FillArea(byte value = 32)
    {
        Array.Fill(_buffer, value);
    }
    
    internal void Insert(byte[] bytes, int offset, int length)
    {
        throw new NotImplementedException();
    }

    //internal void Insert(string text, int offset, int length)
    //{
    //    // initialise with spaces 
    //    Array.Fill(_buffer, (byte)32, offset, length);

    //    // get ascii
    //    var bytes = Encoding.ASCII.GetBytes(text);

    //    // so we don't overwrite field length or area length
    //    var copyLength = Math.Min(bytes.Length, length);
    //    if (offset + copyLength > AreaLength)
    //    {
    //        copyLength = AreaLength - offset + 1;
    //    }

    //    // copy the string to buffer
    //    Buffer.BlockCopy(bytes, 0, _buffer, offset, copyLength);
    //}

    internal void Insert(string text, int offset, int length)
    {
        ArgumentNullException.ThrowIfNull(text);

        // these should never fire - offsets and lengths are
        // checked at compile time
        Debug.Assert(offset >= 0);
        Debug.Assert(length >= 0);
        Debug.Assert(offset + length <= _buffer.Length);

        var field = _buffer.AsSpan(offset, length);

        // Initialise field to spaces.
        field.Fill((byte)' ');

        // Truncate source text to field size.
        var source = text.AsSpan();
        if (source.Length > field.Length)
        {
            source = source[..field.Length];
        }

        Encoding.ASCII.GetBytes(source, field);
    }
    
    internal void Insert(ushort value, int offset)
    {
        //Original
        //var bytes = BitConverter.GetBytes(value);
        //Buffer.BlockCopy(bytes, 0, _buffer, offset, 2);

        // Fastest
        //Unsafe.WriteUnaligned(
        //    ref _buffer[offset],
        //    value);

        // Not quite as fast as the Unsafe one but more readable
        BinaryPrimitives.WriteUInt16LittleEndian(
            _buffer.AsSpan(offset),
            value);
    }

    internal void Insert(uint value, int offset)
    {
        //var bytes = BitConverter.GetBytes(value);
        //Buffer.BlockCopy(bytes, 0, _buffer, offset, 4);

        BinaryPrimitives.WriteUInt32LittleEndian(
            _buffer.AsSpan(offset),
            value);
    }

    internal ushort GetBinaryU2(int offset)
    {
        //return BitConverter.ToUInt16(_buffer, offset);

        return BinaryPrimitives.ReadUInt16LittleEndian(_buffer.AsSpan(offset));
    }

    internal uint GetBinaryU4(int offset)
    {
        //return BitConverter.ToUInt32(_buffer, offset);

        return BinaryPrimitives.ReadUInt32LittleEndian(_buffer.AsSpan(offset));
    }

    internal string GetString(int offset, int length)
    {
        return Encoding.ASCII.GetString(_buffer, offset, length).TrimEnd();
    }

    internal void Write(Stream stream)
    {
        stream.Write(_buffer, 0, _buffer.Length); 
    }

    internal void Write(StreamWriter stream)
    {
        stream.WriteLine(GetString(0, AreaLength));
    }

    internal void Read(StreamReader stream)
    {
        FillArea();

        var text = stream.ReadLine();
        if (text == null)
        {
            Expressions.Indicators.IsEof = true;
        }
        else
        {
            Expressions.Indicators.IsEof = false;
            Insert(text, 0, text.Length);
        }
    }

    internal void CopyToBuffer(byte[] objectBuffer, int objectOffset, int sourceOffset, int sourceLength)
    {
        Buffer.BlockCopy(_buffer, sourceOffset, objectBuffer, objectOffset, sourceLength);
    }

    internal int CompareWithBuffer(byte[] sourceBuffer, int sourceOffset, int offset, int length)
    {
        for(var i = sourceOffset; i < length; ++i)
        {
            if (sourceBuffer[i] < _buffer[offset])
                return -1;

            if (sourceBuffer[i] > _buffer[offset])
                return 1;

            ++offset;
        }

        return 0;
    }

    internal void Increment<T>(int offset) where T : unmanaged, IUnsignedNumber<T>
    {
        ref var value = ref Unsafe.As<byte, T>(ref _buffer[offset]);
        value++;
    }
}