using System.Collections.Generic;

namespace Tyke.Net.Data;

internal static class BlockCache
{
    private static readonly Dictionary<int, byte[]> _cache = new();

    internal static byte[] GetBlock(int length)
    {
        if(_cache.TryGetValue(length, out var block))
            return block;

        var temp = new byte[length];

        for (int i = 0; i < length; ++i)
            temp[i] = 32;

        _cache.Add(length, temp);

        return temp;
    }
}