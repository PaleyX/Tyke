using System;

namespace Tyke.Net.Data
{
    internal abstract class DatafieldBase : Symbols.SymbolBase
    {
        private readonly DataBuffer _area;
        private int _offset;
        private int _length;
        private readonly DatafieldTypes _type;

        [Flags]
	    private enum Elements
        {
            None = 0,
    		Offset = 1,
	    	Length = 2,
		    Type   = 4
	    };

        internal enum DatafieldTypes
        {
            Binary,
            Alpha
        }
        
        private Elements _elementsSet = Elements.None;

        internal DatafieldBase(DataBuffer area, string name, string offset, string length, DatafieldTypes type)
        {
            _area = area;
            _type = type; 

            Symbols.SymbolTable.AddSymbol(name, this);
            SetOffset(offset);
            SetLength(length);
        }

        internal int Offset => _offset;
        internal int Length => _length;
        protected DataBuffer Area => _area;
        internal DatafieldTypes DatafieldType => _type;

        internal void CopyToBuffer(byte[] buffer, int offset)
        {
            Area.CopyToBuffer(buffer, offset, Offset, Length);
        }

        internal int CompareWithBuffer(byte[] buffer, int offset)
        {
            return Area.CompareWithBuffer(buffer, offset, Offset, Length);
        }

        internal bool CheckAndValidateElements()
        {
	        bool result = true;

            // make sure everything is set
            if (string.IsNullOrWhiteSpace(Name))
                Errors.Error.SyntaxError("No name set");

            if (!IsSet(Elements.Offset))
                Errors.Error.SyntaxError("No offset set");

            if (!IsSet(Elements.Length))
                Errors.Error.SyntaxError("No length set");

            // area at least must be valid
            if (!_area.IsValidArea())
            {
                Errors.Error.SyntaxError("Invalid area");
                return false;
            }

            // field size > 0
            if (_length < 1)
            {
                Errors.Error.SyntaxError("Invalid field length");
                result = false;
            }

            // offset
            if (_offset < 0)
            {
                Errors.Error.SyntaxError("Invalid field start");
                result = false;
            }

            // does field size exceed area
            if (_offset + _length > _area.AreaLength)
            {
                Errors.Error.ReportError("Field overflows area");
                result = false;
            }

            // Validate lengths
            if (!ValidateFieldLengthForType())
            {
                result = false;
            }

	        return result;
        }

        internal abstract void SetConstant(string constant);
        internal abstract bool CanCast(DatafieldBase datafield);
        internal abstract void Cast(DatafieldBase datafield);
        internal abstract void Set(string value);
        protected abstract bool ValidateFieldLengthForType();

        private void SetOffset(string value)
        {
            _offset = Tools.StringTools.GetPositiveNumber(value);
            _elementsSet |= Elements.Offset;

            // actual offset
            _offset -= 1;
        }

        private void SetLength(string value)
        {
            _length = Tools.StringTools.GetPositiveNumber(value);
            _elementsSet |= Elements.Length;
        }

        private bool IsSet(Elements flag)
        {
            return (_elementsSet & flag) == flag;
        }
    }
}
