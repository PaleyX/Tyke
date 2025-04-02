using System.Diagnostics;

namespace Tyke.Net.Data
{
    internal class DatafieldAlpha : DatafieldBase
    {
        internal DatafieldAlpha(DataBuffer area, string name, string offset, string length)
            : base(area, name, offset, length, DatafieldTypes.Alpha)
        {
        }

        protected override bool ValidateFieldLengthForType()
        {
            return true;
        }

        internal override void SetConstant(string constant)
        {
            if(Tools.StringTools.IsQuotedString(constant))
            {
                string text = Tools.StringTools.GetQuotedString(constant);
                if(text.Length > Length)
                    Errors.Error.ReportError("Constant is longer than field");
                Set(text);
            }
            else
                Errors.Error.SyntaxError("Invalid constant");
        }

        internal void Set(OperandAlpha operand)
        {
            Set(operand.ToString());
        }

        internal override void Set(string value)
        {
            Area.Insert(value, Offset, Length);
        }

        internal void Set(DatafieldAlpha datafield)
        {
            //TODO: Do a buffer to buffer copy rather than convert to string first
            Set(datafield.ToString());
        }

        internal override bool CanCast(DatafieldBase datafield)
        {
            return datafield is DatafieldBinary;
        }

        internal override void Cast(DatafieldBase datafield)
        {
            var x = datafield as DatafieldBinary;
            Debug.Assert(x != null);

            var source = ToString();

            if (Tools.StringTools.CanConvert<uint>(source))
            {
                uint value = Tools.StringTools.Convert<uint>(ToString(), false);
                x.Set(value);
            }
            else
                Errors.Error.ReportError("Cannot cast value");
        }

        public override string ToString()
        {
            return Area.GetString(Offset, Length);
        }
    }
}
