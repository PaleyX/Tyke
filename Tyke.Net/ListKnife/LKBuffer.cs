using System;
using System.Diagnostics;

namespace Tyke.Net.ListKnife
{
    internal class LkBuffer
    {
        private int _startUrn;
        private int _endUrn;
        private readonly byte[] _buffer;
        private readonly int _length;

        internal LkBuffer(int startUrn, int length)
        {
            _length = length;

            Debug.Assert(IsValidBitmapBufferLength(_length));

            _startUrn = CalcStartUrn(startUrn);
            _endUrn = CalcEndUrn(_startUrn);

            _buffer = new byte[_length];
        }

        internal bool Overflow(int urn)
        {
            Debug.Assert(_startUrn >= 0);
            Debug.Assert(_endUrn >= 0);

            return urn > _endUrn;
        }

        internal static bool IsValidBitmapBufferLength(int length, bool reportError = true) 
        {
            bool result = length is >= 100 and <= 10000;

            if (result == false && reportError)
                Errors.Error.ReportError("Invalid bitmap length, must be between 100 and 10,000");

            return result;
        }

        internal bool AddUrn(int urn)
        {
            Debug.Assert(urn <= _endUrn);

            int offset = (urn - _startUrn) / 8;
            int bit = (int)(urn - _startUrn) % 8;

            Debug.Assert(offset >= 0 && offset < _length);

            // don't bother if already set
            if ((_buffer[offset] & (1 << bit)) > 0)
                return false;

            _buffer[offset] |= (byte)(1 << bit);

            return true;
        }

        internal void Clear(int startUrn)
        {
            _startUrn = CalcStartUrn(startUrn);
            _endUrn = CalcEndUrn(_startUrn);

            // profiled and is very fast 
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        internal byte[] Buffer => _buffer;
        internal int StartUrn => _startUrn;
        internal int EndUrn => _endUrn;

        private int CalcStartUrn(int urn)
        {
            if (urn <= 7)
                return 0;

            if (urn % 8 == 0)
                return urn;

            return urn - (urn % 8);
        }

        private int CalcEndUrn(int urn)
        {
            return urn + (_length * 8) - 1;
        }
    }
}
