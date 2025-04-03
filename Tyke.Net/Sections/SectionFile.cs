using System;
using System.Diagnostics;
using System.IO;

namespace Tyke.Net.Sections;

internal class SectionFile : SectionBufferBase
{
    private Stream _stream;
    private StreamWriter _textWriter;
    private StreamReader _textReader;

    private int _read;
    private int _write;
    private int _open;
    private int _close;

    private enum AccessTypes
    {
        Undefined,
        Read,
        Write
    }

    private enum FileTypes
    {
        Undefined,
        Text,
        Binary
    }

    private AccessTypes _accessType = AccessTypes.Undefined;
    private FileTypes _fileType = FileTypes.Undefined;
    private Data.OperandAlpha _pathname;

    internal SectionFile()
        : base(SectionTypes.File)
    {
        LinesParser = new SectionLines
        (
            this,
            new SectionLine("name", ParseName),
            new SectionLine("length", ParseLength),
            new SectionLine("access", ParseAccess),
            new SectionLine("pathname", ParsePathname),
            new SectionLine("type", ParseType)
        );
    }

    internal override bool CanOpen() { return true; }
    internal override bool CanRead() { return _accessType == AccessTypes.Read; }
    internal override bool CanWrite() { return _accessType == AccessTypes.Write; }
    internal override bool CanClose() { return true; }

    internal override void Process(SectionActions action)
    {
        switch (action)
        {
            case SectionActions.Read:
                ReadMe();
                break;
            case SectionActions.Write:
                WriteMe();
                break;
            case SectionActions.Open:
                OpenMe();
                break;
            case SectionActions.Close:
                CloseMe();
                break;
            default:
                throw new ApplicationException("Unsupported action in File");
        }
    }

    internal override void ReportStatistics()
    {
        //TODO: When multiple file (different pathnames) have been opened

        var stats = new Statistics("File", this);

        stats.Add("Pathname", _pathname.ToString());
        stats.Add("Opened", _open);
        stats.Add("Read", _read);
        stats.Add("Written", _write);
        stats.Add("Closed", _close);

        stats.Report();
    }

    #region Parsing
    protected void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }

    protected void ParseAccess(string value)
    {
        switch(value)
        {
            case "read":
                _accessType = AccessTypes.Read;
                break;
            case "write":
                _accessType = AccessTypes.Write;
                break;
            default:
                Errors.Error.SyntaxError(Errors.StdErrors.UnknownOption);
                break;
        }
    }

    protected void ParsePathname(string value)
    {
        _pathname = new Data.OperandAlpha();
        _pathname.SetFromToken(value);
    }

    protected void ParseType(string value)
    {
        switch (value)
        {
            case "text":
                _fileType = FileTypes.Text;
                break;
            case "binary":
                _fileType = FileTypes.Binary;
                break;
            default:
                Errors.Error.SyntaxError(Errors.StdErrors.UnknownOption);
                break;
        }
    }
    #endregion Parsing

    private void OpenMe()
    {
        string pathname = _pathname.ToString();

        switch (_accessType)
        {
            case AccessTypes.Read:
                _stream = new FileStream(pathname, FileMode.Open, FileAccess.Read, FileShare.None);
                if (_fileType == FileTypes.Text)
                {
                    _textReader = new StreamReader(_stream);
                }
                break;
            case AccessTypes.Write:
                _stream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                if (_fileType == FileTypes.Text)
                {
                    _textWriter = new StreamWriter(_stream);
                }
                break;
            default:
                throw new ApplicationException("Unknown access type opening file");
        }

        Expressions.Indicators.IsEof = false;
        ++_open;
    }

    private void CloseMe()
    {
        ++_close;

        if (_stream == null)
            throw new ApplicationException("Attempt to close unopen file");

        if (_textReader != null)
        {
            _textReader.Close();
            _textReader = null;
            _stream = null;
            return;
        }

        if (_textWriter != null)
        {
            _textWriter.Close();
            _textWriter = null;
            _stream = null;
            return;
        }
            
        _stream.Close();
        _stream = null;
    }

    private void WriteMe()
    {
        if (_stream == null)
            throw new ApplicationException("Attempt to write to unopen file");

        Debug.Assert(_accessType == AccessTypes.Write);

        ++_write;

        switch (_fileType)
        {
            case FileTypes.Binary:
                Buffer.Write(_stream);
                return;
            case FileTypes.Text:
                Buffer.Write(_textWriter);
                return;
            default:
                throw new ApplicationException("unknown file type when writing");
        }
    }

    private void ReadMe()
    {
        if (_stream == null)
            throw new ApplicationException("Attempt to read from unopen file");

        Debug.Assert(_accessType == AccessTypes.Read);

        ++_read;

        switch (_fileType)
        {
            //TODO: Binary
            case FileTypes.Text:
                Buffer.Read(_textReader);
                return;
            default:
                throw new ApplicationException("unknown file type when reading");
        }
    }
}