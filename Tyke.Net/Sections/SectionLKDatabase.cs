using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

namespace Tyke.Net.Sections
{
    internal class SectionLkDatabase : SectionBase
    {
        private string _directory;
        private string _prefix;
        private Data.DatafieldBinary _reference;
        private int _bitmapLength;
        private string _bitmapVersion;
        private string _databaseVersion;
        private int _fileSplit;
        private string _description;
        private int _currentAttrId = StartAttrId;

        private int _permUrn;
        private int _maxPermUrn;
        private int _tempUrn;
        private long _start = 1;

        private readonly Dictionary<int, SectionLkAttribute> _attributes = new();
        private readonly Dictionary<int, SectionLkFilter> _filters = new();

        private readonly Dictionary<ListKnife.AttributeValuePair, ListKnife.LkToken> _tokens = new();

        private Stream _tempStream;
        private Stream _dataStream;
        private Stream _transStream;

        private BinaryWriter _tempWriter;
        private BinaryWriter _dataWriter;
        private BinaryWriter _transWriter;

        private const int UniverseAttrId = 0;
        private const int FilterAttrId = 1;
        private const int StartAttrId = 2;

        internal SectionLkDatabase()
            : base(SectionTypes.LkDatabase)
        {
            LinesParser = new SectionLines
            (
                this,
                new SectionLine("name", ParseName),
                new SectionLine("directory", ParseDirectory),
                new SectionLine("file name prefix", ParseFileNamePrefix),
                new SectionLine("reference field", ParseReferenceField),
                new SectionLine("bitmap length", ParseBitmapLength),
                new SectionLine("bitmap version", ParseBitmapVersion),
                new SectionLine("database version", ParseDatabaseVersion),
                new SectionLine("listknife description", ParseListknifeDescription),
                new SectionLine("file split", ParseFileSplit)
            );
        }

        internal override void Process(SectionActions action)
        {
            throw new NotImplementedException();
        }

        internal override void ReportStatistics()
        {
            //TODO: Statistics
        }

        internal int RegisterAttribute(SectionLkAttribute attribute)
        {
            Debug.Assert(attribute != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(attribute.Description));

            var x = _attributes.Values.FirstOrDefault(v => string.Compare(v.Description, attribute.Description, true) == 0);
            if (x != null)
            {
                Errors.Error.ReportError("Duplicate attribute description");
                return 0;
            }

            _attributes.Add(_currentAttrId, attribute);

            return _currentAttrId++;
        }

        internal int RegisterFilter(SectionLkFilter filter)
        {
            Debug.Assert(filter != null);

            int id = _filters.Count;

            var x = _filters.Values.FirstOrDefault(v => string.Compare(v.Description, filter.Description, true) == 0);
            if (x != null)
            {
                Errors.Error.ReportError("Duplicate filter description");
                return int.MinValue;
            }

            _filters.Add(id, filter);

            return id;
        }

        internal void AddAttributeValue(int attributeId, string value)
        {
            int urn = Convert.ToInt32(_reference.GetDWord());
            Universe(urn);

            AddToken(attributeId, value);
        }

        internal void AddFilterFlag(int filterId)
        {
            int urn = Convert.ToInt32(_reference.GetDWord());
            Universe(urn);

            AddToken(FilterAttrId, _filters[filterId].Description);
        }

        private void Universe(int urn)
        {
            if (urn != _permUrn)
            {
                SetUrn(urn);
                AddToken(UniverseAttrId, "Universe");
            }
        }

        internal int TempUrn => _tempUrn;

        internal int BitmapLength => _bitmapLength;

        internal BinaryWriter TempWriter => _tempWriter;
        internal BinaryWriter DataWriter => _dataWriter;

        private string TempPathname { get; set; }
        private string DataPathname { get; set; }
        private string TransPathname { get; set; }

        private void AddToken(int attributeId, string value)
        {
            var pair = new ListKnife.AttributeValuePair(attributeId, value);

            if (!_tokens.TryGetValue(pair, out var token))
            {
                token = new ListKnife.LkToken(this);
                _tokens.Add(pair, token);
            }

            token.AddPerson();
        }

        private void SetUrn(int permUrn)
        {
            if (permUrn == _permUrn)
                return;

            ++_tempUrn;
            _permUrn = permUrn;

            _maxPermUrn = Math.Max(_maxPermUrn, _permUrn);

            // add to translation vector
            _transWriter.Write(_permUrn);

            if (_tempUrn % 10000 == 0)
                ProcessOrphans();
        }

        private void ProcessOrphans()
        {
            // make sure everything is written out
            foreach (var pair in _tokens)
            {
                if (pair.Value.IsOrphaned(_tempUrn))
                    pair.Value.RemoveOrphan();
            }
        }

        #region parsing
        private void ParseName(string value)
        {
            Symbols.SymbolTable.AddSymbol(value, this);
        }

        private void ParseDirectory(string value)
        {
            _directory = Tools.StringTools.GetQuotedString(value);
            Tools.StringTools.ValidateNonEmpty(_directory);
        }

        private void ParseFileNamePrefix(string value)
        {
            _prefix = Tools.StringTools.GetQuotedString(value);
            Tools.StringTools.ValidateOneWord(_prefix);
        }

        private void ParseReferenceField(string value)
        {
            _reference = Symbols.SymbolTable.GetSymbol<Data.DatafieldBinary>(value);
        }

        private void ParseBitmapLength(string value)
        {
            _bitmapLength = Tools.StringTools.GetPositiveNumber(value);
            ListKnife.LkBuffer.IsValidBitmapBufferLength(_bitmapLength);
        }

        private void ParseBitmapVersion(string value)
        {
            _bitmapVersion = Tools.StringTools.GetQuotedString(value);
            Tools.StringTools.ValidateNonEmpty(_bitmapVersion);
        }

        private void ParseDatabaseVersion(string value)
        {
            _databaseVersion = Tools.StringTools.GetQuotedString(value);
            Tools.StringTools.ValidateNonEmpty(_databaseVersion);
        }

        private void ParseFileSplit(string value)
        {
            _fileSplit = Tools.StringTools.GetPositiveNumber(value);
            if (_fileSplit < 500)
                Errors.Error.SyntaxError("File Split cannot be less that 500");
        }

        private void ParseListknifeDescription(string value)
        {
            _description = Tools.StringTools.GetQuotedString(value);
            Tools.StringTools.ValidateNonEmpty(_description);
        }
        #endregion parsing

        internal override void PreProcess()
        {
            // create directory
            if (Directory.Exists(_directory))
            {
                //TODO: remove the directory delete
                //// for testing
                Directory.Delete(_directory, true);
                Console.WriteLine("---- DELETING DIRECTORY FOR TESTING ----");
                ////
                //throw new TykeRunTimeException("ListKnife directory already exists");
            }

            Directory.CreateDirectory(_directory);

            CreateListknifeFiles();
        }

        private void CreateListknifeFiles()
        {
            // temp data file
            try
            {
                TempPathname = Path.Combine(_directory, "temp.dat");
                _tempStream = new FileStream(TempPathname, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                _tempWriter = new BinaryWriter(_tempStream);
            }
            catch (Exception e)
            {
                throw new TykeRunTimeException("Error opening temp file: " + e.Message, Name);
            }

            // real data file
            try
            {
                DataPathname = Path.Combine(_directory, _prefix + "bm.dat");
                _dataStream = new FileStream(DataPathname, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                _dataWriter = new BinaryWriter(_dataStream);
            }
            catch (Exception e)
            {
                throw new TykeRunTimeException("Error opening data file: " + e.Message, Name);
            }

            // trans file
            try
            {
                TransPathname = Path.Combine(_directory, "trans.dat");
                _transStream = new FileStream(TransPathname, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                _transWriter = new BinaryWriter(_transStream);
            }
            catch (Exception e)
            {
                throw new TykeRunTimeException("Error opening trans file: " + e.Message, Name);
            }
        }

        internal override void PostProcess()
        {
            try
            {
                Consolidate();
            }
            catch (Exception e)
            {
                throw new TykeRunTimeException("Error consolidating database: " + e.Message, Name);
            }
        }

        #region Consolidation
        private void Consolidate()
        {
            // Finish any writing
            foreach (var item in _tokens.Values)
            {
                item.ForceDump();
            }

            // order
            var sorted = _tokens.Select(t => t.Key)
                .OrderBy(x => x.AttributeId)
                .ThenBy(x => x.Value);

            // Close and flush temp file
            _tempWriter.Flush();
            _tempWriter.Close();

            HashSet<int> added = [];

            // start xml documents
            XDocument attributesXml = new XDocument(
                new XElement("Attributes", null));

            XDocument valuesXml = new XDocument(
                new XElement("Values", null));

            using (Stream stream = new FileStream(TempPathname, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (var reader = new BinaryReader(stream))
                {
                    foreach (var tokenKey in sorted)
                    {
                        ListKnife.LkToken token = _tokens[tokenKey];
                        token.AppendData(reader, _dataWriter);

                        switch (tokenKey.AttributeId)
                        {
                            case UniverseAttrId:
                                {
                                    attributesXml.Element("Attributes").Add(GetAttributeElement(tokenKey.AttributeId, "Universe", null, "A", false));
                                    valuesXml.Element("Values").Add(GetValueElement(token, tokenKey.AttributeId, "Everybody", "Everybody"));
                                }
                                break;
                            case FilterAttrId:
                                {
                                    if (!added.Contains(tokenKey.AttributeId))
                                    {
                                        attributesXml.Element("Attributes").Add(GetAttributeElement(tokenKey.AttributeId, "Filter", null, "A", true));
                                        added.Add(tokenKey.AttributeId);
                                    }

                                    valuesXml.Element("Values").Add(GetValueElement(token, tokenKey.AttributeId, tokenKey.Value, tokenKey.Value));
                                }
                                break;
                            default:
                                {
                                    SectionLkAttribute attribute = _attributes[tokenKey.AttributeId];

                                    if (!added.Contains(tokenKey.AttributeId))
                                    {
                                        attributesXml.Element("Attributes").Add(GetAttributeElement(tokenKey.AttributeId, attribute.Description, attribute.Group, attribute.TypeCode, false));
                                        added.Add(tokenKey.AttributeId);
                                    }

                                    valuesXml.Element("Values").Add(GetValueElement(token, tokenKey.AttributeId, tokenKey.Value, attribute.GetValueDescription(tokenKey.Value)));
                                }
                                break;
                        }
                    }
                }
            }

            // close files
            _dataWriter.Flush();
            _dataWriter.Close();

            _transWriter.Flush();
            _transWriter.Close();

            // write out xml
            attributesXml.Save(Path.Combine(_directory, "attributes.xml"));
            valuesXml.Save(Path.Combine(_directory, "values.xml"));

            // Translation files
            using (var translators = new ListKnife.Translators(TransPathname, _directory))
            {
                translators.CreateTemp2Perm();
                translators.CreatePerm2Temp();
            }
        }

        private XElement GetAttributeElement(int attributeId, string name, string group, string type, bool system)
        {
            group ??= string.Empty;

            return new XElement("Attribute",
                        new XAttribute("id", attributeId),
                        new XAttribute("name", name),
                        new XAttribute("group", group),
                        new XAttribute("type", type),
                        new XAttribute("system", system));
        }

        private XElement GetValueElement(ListKnife.LkToken token, int attributeId, string value, string description)
        {
            ArgumentNullException.ThrowIfNull(value, "Response value cannot be null or empty");
            ArgumentNullException.ThrowIfNull(description, "Response description cannot be null or empty");

            XElement element = new XElement("Value",
                                    new XAttribute("id", attributeId),
                                    new XAttribute("value", value),
                                    new XAttribute("description", description),
                                    new XAttribute("tally", token.Tally),
                                    new XAttribute("start", _start),
                                    new XAttribute("number", token.BufferTally),
                                    new XAttribute("offset", token.StartOffset));

            _start += token.BufferTally;

            return element;
        }
        #endregion
    }
}
