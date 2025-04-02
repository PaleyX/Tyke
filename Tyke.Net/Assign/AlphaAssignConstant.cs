namespace Tyke.Net.Assign
{
    internal class AlphaAssignConstant : AlphaAssignBase
    {
        internal AlphaAssignConstant(string element)
        {
            Constant = Tools.StringTools.GetQuotedString(element);
        }

        internal string Constant { get; private set; }    }
}
