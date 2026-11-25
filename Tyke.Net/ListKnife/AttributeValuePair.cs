using System;

namespace Tyke.Net.ListKnife;

internal sealed class AttributeValuePair : IEquatable<AttributeValuePair>
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
        return HashCode.Combine(AttributeId, Value);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AttributeValuePair)obj);
    }

    public bool Equals(AttributeValuePair other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return AttributeId == other.AttributeId && Value == other.Value;
    }
}