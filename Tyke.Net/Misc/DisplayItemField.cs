namespace Tyke.Net.Misc
{
    internal class DisplayItemField : DisplayItemBase
    {
        private readonly Data.DatafieldBase _datafield;

        internal DisplayItemField(Data.DatafieldBase datafield)
        {
            _datafield = datafield;
        }

        internal override string Value()
        {
            return _datafield.ToString();
        }
    }
}
