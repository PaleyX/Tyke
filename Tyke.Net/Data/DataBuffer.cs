using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

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

    internal bool IsValidArea()
    {
        return _areaLength > 0;
    }

    internal int AreaLength => _areaLength;

    internal void FillArea(byte value = 32)
    {
        if (value == 32)
        {
            var block = BlockCache.GetBlock(AreaLength);
            Buffer.BlockCopy(block, 0, _buffer, 0, AreaLength);
        }
        else
        {
            for (var i = 0; i < _areaLength; ++i)
            {
                _buffer[i] = value;
            }
        }
    }

    internal void Insert(byte[] bytes, int offset, int length)
    {
        throw new NotImplementedException();
    }

    internal void Insert(string text, int offset, int length)
    {
        // initialise with spaces 
        var block = BlockCache.GetBlock(length);
        Debug.Assert(block.Length == length);
        Buffer.BlockCopy(block, 0, _buffer, offset, length);

        // get ascii
        var bytes = Encoding.ASCII.GetBytes(text);

        // so we don't overwrite field length or area length
        var copyLength = Math.Min(bytes.Length, length);
        if (offset + copyLength > AreaLength)
        {
            copyLength = AreaLength - offset + 1;
        }

        // copy the string to buffer
        Buffer.BlockCopy(bytes, 0, _buffer, offset, copyLength);
    }

    internal void Insert(ushort value, int offset)
    {
        var bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, _buffer, offset, 2);
    }

    internal void Insert(uint value, int offset)
    {
        var bytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, _buffer, offset, 4);
    }

    internal ushort GetBinaryU2(int offset)
    {
        return BitConverter.ToUInt16(_buffer, offset);
    }

    internal uint GetBinaryU4(int offset)
    {
        return BitConverter.ToUInt32(_buffer, offset);
    }

    internal string GetString(int offset, int length)
    {
        // get trimmed length
        var i = length + offset;
        while (i >= offset)
        {
            if (_buffer[i] != 32)
                break;

            --i;
        }

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
        unsafe
        {
            fixed (byte* p = _buffer)
            {
                (*(T*)(p + offset))++;
            }
        }
    }
}