using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Tyke.Net.Sections
{
    internal class SectionSqlDatabase : SectionBase
    {
        private string _connectionString;
        private SqlConnection _connection;
        private int _refCount;

        internal SectionSqlDatabase()
            : base(SectionTypes.SqlDatabase)
        {
            LinesParser = new SectionLines
            (
                this,
                new SectionLine("name", ParseName),
                new SectionLine("connection string", ParseConnectionString)
            );
        }

        internal override void Process(SectionActions action)
        {
           // nowt to do
            throw new TykeRunTimeException("Cannot perform action on SqlDatabase", Name);
        }

        internal override void ReportStatistics()
        {
            // Nowt
        }

        internal SqlConnection GetConnection()
        {
            if (_refCount == 0)
            {
                Debug.Assert(_connection == null);

                try
                {
                    _connection = new SqlConnection(_connectionString);
                    _connection.Open();
                }
                catch (Exception e)
                {
                    throw new TykeRunTimeException("Error opening SqlDatabase connection" + e.Message, Name);
                }
            }

            ++_refCount;

            return _connection;
        }

        internal void ReleaseConnection()
        {
            if (_refCount <= 0)
                throw new TykeRunTimeException("Ref count error releasing connection", Name);

            --_refCount;

            if (_refCount == 0)
            {
                _connection.Close();
                _connection = null;
            }
        }

        #region Parsing
        protected void ParseName(string value)
        {
            Symbols.SymbolTable.AddSymbol(value, this);
        }

        private void ParseConnectionString(string value)
        {
            _connectionString = Tools.StringTools.GetQuotedString(value);
        }
        #endregion Parsing
    }
}
