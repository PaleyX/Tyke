namespace Tyke.Net.Assign
{
    internal class BinaryAssignConstant : BinaryAssignBase
    {
        private readonly uint _constant;

        internal BinaryAssignConstant(string element)
        {
            _constant = Tools.StringTools.Convert<uint>(element);
        }

        internal uint GetDWord()
        {
            return _constant;
        }
    }
}
