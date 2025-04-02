using System;
using System.Collections.Generic;
using System.IO;

namespace Tyke.Net.ListKnife
{
    internal class LkToken
    {
        private readonly Sections.SectionLkDatabase _database;
        private LkBuffer _buffer;
        private int _tally;
        private long _offset;

        private const int Tmpbufscale = 3;
        private int _bufferTally;

        private readonly List<long> _offsets = [];

        internal LkToken(Sections.SectionLkDatabase database)
        {
            _database = database;
        }

        internal int Tally => _tally;

        internal int BufferTally => _bufferTally;

        internal long StartOffset => _offset;

        internal void AddPerson()
        {
            if (_buffer == null)
            {
                _buffer = new LkBuffer(_database.TempUrn, _database.BitmapLength);
            }
            else
            {
                if (_buffer.Overflow(_database.TempUrn))
                {
                    DumpBuffer();
                    _buffer.Clear(_database.TempUrn);
                }
            }

            if (_buffer.AddUrn(_database.TempUrn))
                ++_tally;
        }


        internal void AppendData(BinaryReader reader, BinaryWriter writer)
        {
            // move to end of data file
            _offset = writer.Seek(0, SeekOrigin.End);

            foreach (long offset in _offsets)
            {
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                // read header
                Int32 valueId = reader.ReadInt32();
                Int32 length = reader.ReadInt32();
                Int32 urnStart = reader.ReadInt32();
                Int32 urnEnd = reader.ReadInt32();
                Int32 uncompressedLength = reader.ReadInt32();

                // write header 
                writer.Write(valueId);
                writer.Write(length);
                writer.Write(urnStart);
                writer.Write(urnEnd);
                writer.Write(uncompressedLength);
                writer.Flush();

                // Buffer
                byte[] buffer = reader.ReadBytes(length);
                if (buffer.Length != length)
                    throw new ApplicationException("Bad read apending data");
                writer.Write(buffer);
            }
        }

        internal void DumpBuffer()
        {
            ++_bufferTally;

            // move to end of temp file, get the offset and store it
            long offset = _database.TempWriter.Seek(0, SeekOrigin.End);
            _offsets.Add(offset);

            int writeLength = 0;
            byte[] buffer;

            // compression
            byte[] tempBuffer = new byte[_database.BitmapLength * Tmpbufscale];

            int compressionLength = CompressIt(_buffer.Buffer, tempBuffer);

            // how did it compress? Which are we using
            if (compressionLength <= _database.BitmapLength - 50)
            {
                writeLength = compressionLength;
                buffer = tempBuffer;
            }
            else
            {
                writeLength = _database.BitmapLength;
                buffer = _buffer.Buffer;
            }

            // bitmap header
            _database.TempWriter.Write((int)1);
            _database.TempWriter.Write((int)writeLength);
            _database.TempWriter.Write((int)_buffer.StartUrn);
            _database.TempWriter.Write((int)_buffer.EndUrn);
            _database.TempWriter.Write((int)_database.BitmapLength);

            _database.TempWriter.Write(buffer, 0, writeLength);
        }

        internal void ForceDump()
        {
            if (_buffer != null)
                DumpBuffer();
        }

        internal bool IsOrphaned(int tempUrn)
        {
            if (_buffer == null)
                return false;

            return _buffer.Overflow(tempUrn);
        }

        internal void RemoveOrphan()
        {
            ForceDump();
            _buffer = null;
        }

        private int CompressIt(byte[] pin, byte[] pout)
        {
            int sequence = 0;

            // get used length
            int i = pin.Length - 1;
            int usedLength = _database.BitmapLength;

            while (i >= 0)
            {
                if (pin[i] != 0)
                    break;

                --usedLength;
                --i;
            }

            // Compress away
            i = 0;
            int o = 0;
            while(i < usedLength)
            {
                if (i < usedLength && pin[i] == 255)
                {
                    sequence = 0;
                    while (i < usedLength && pin[i] == 255)
                    {
                        ++i;
                        if (++sequence == 255)
                            break;
                    }
                    pout[o++] = 255;
                    pout[o++] = Convert.ToByte(sequence);
                }

                if (i < usedLength && pin[i] == 0)
                {
                    sequence = 0;
                    while (i < usedLength && pin[i] == 0)
                    {
                        ++i;
                        if (++sequence == 255)
                            break;
                    }
                    pout[o++] = 0;
                    pout[o++] = Convert.ToByte(sequence);
                }

                if (i < usedLength && pin[i] != 0 && pin[i] != 255)
                {
                    pout[o++] = pin[i++];
                }
            }

            return o;
        }
    }
}
