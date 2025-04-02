using System;
using System.Data.SqlClient;

namespace Tyke.Net.Data
{
    internal class DatabaseColumn
    {
        private enum ColumnTypes
        {
            Undefined,
            Int,
            String
        }

        private readonly DatafieldBase _datafield;

        private int _ordinal;
        private int _size;
        private ColumnTypes _type;

        internal DatabaseColumn(DatafieldBase datafield, string mappedto)
        {
            _datafield = datafield;
            MappedTo = mappedto;

            IsMapped = false;
        }

        internal string MappedTo { get; }

        internal bool IsMapped
        {
            get;
            private set;
        }

        internal void SetMapping(int ordinal, int size, string type)
        {
            IsMapped = true;

            _ordinal = ordinal;
            _size = size;

            switch (type)
            {
                case "int":
                    _type = ColumnTypes.Int;
                    break;
                case "nvarchar":
                    _type = ColumnTypes.String;
                    break;
                case "nchar":
                    _type = ColumnTypes.String;
                    break;
                default:
                    _type = ColumnTypes.Undefined;
                    Errors.Error.SyntaxError("Unsupported data type: " + type);
                    break;
            }

            if (_type != ColumnTypes.Undefined)
            {
                if (_type == ColumnTypes.Int)
                {
                    if (_datafield is not DatafieldBinary)
                    {
                        Errors.Error.ReportError("Cannot map database type to datafield type on {0}", _datafield.Name);
                    }

                    return;
                }

                if (_type == ColumnTypes.String)
                {
                    if (_datafield is not DatafieldAlpha)
                    {
                        Errors.Error.ReportError("Cannot map database type to datafield type on {0}", _datafield.Name);
                    }

                    return;
                }

                throw new TykeRunTimeException("Unknown type mapping datafields");
            }
        }

        internal void Set(SqlDataReader reader)
        {
            if (_type == ColumnTypes.String)
            {
                var x = _datafield as DatafieldAlpha;

                if (reader.IsDBNull(_ordinal))
                    x.Set("");
                else
                    x.Set(reader.GetString(_ordinal));

                return;
            }

            if (_type == ColumnTypes.Int)
            {
                var x = _datafield as DatafieldBinary;

                if (reader.IsDBNull(_ordinal))
                    x.SetMax();
                else
                    x.Set(Convert.ToUInt32(reader[_ordinal]));

                return;
            }

            throw new TykeRunTimeException("Unknown type setting database field");
        }
    }
}
