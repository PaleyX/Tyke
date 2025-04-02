namespace Tyke.Net.Assign
{
    internal class AlphaAssignDatafield : AlphaAssignBase
    {
        internal AlphaAssignDatafield(Data.DatafieldAlpha datafield)
        {
            Datafield = datafield;
        }

        internal Data.DatafieldAlpha Datafield { get; private set; }
    }
}
