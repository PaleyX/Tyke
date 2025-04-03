namespace Tyke.Net.Sections;

internal abstract class SectionBufferBase : SectionBase
{
    protected Data.DataBuffer Buffer;

    internal SectionBufferBase(SectionTypes type)
        : base(type)
    {
    }

    protected void ParseLength(string value)
    {
        int length = Tools.StringTools.GetPositiveNumber(value);
        if (length > 0)
        {
            Buffer = new Data.DataBuffer(length);
        }
    }
}