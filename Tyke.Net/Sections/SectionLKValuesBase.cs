namespace Tyke.Net.Sections;

internal abstract class SectionLkValuesBase : SectionBase
{
    internal SectionLkValuesBase(SectionTypes sectionType)
        : base(sectionType)
    {
    }

    internal abstract string GetDescription(string value);

    internal override void ReportStatistics()
    {
        // nowt
    }
}