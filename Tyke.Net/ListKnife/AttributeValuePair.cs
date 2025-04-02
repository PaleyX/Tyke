namespace Tyke.Net.ListKnife
{
    internal class AttributeValuePair
    {
        internal AttributeValuePair(int attributeId, string value)
        {
            AttributeId = attributeId;
            Value = value;
        }

        internal int AttributeId { get; }
        internal string Value { get; }

        public override int GetHashCode()
        {
            return AttributeId.GetHashCode() ^ Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not AttributeValuePair x)
                return false;

            return x.AttributeId == AttributeId && x.Value == Value;
        }
    }
}
