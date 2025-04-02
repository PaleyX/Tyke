namespace Tyke.Net.Assign
{
    internal class BinaryAssignDatafield : BinaryAssignBase
    {
        private readonly Data.DatafieldBinary _datafield;

        internal BinaryAssignDatafield(Data.DatafieldBinary datafield)
        {
            _datafield = datafield;
        }

        internal uint GetDWord()
        {
            return _datafield.GetDWord();
        }
    }
}
