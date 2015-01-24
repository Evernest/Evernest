using System;
using System.Collections.Generic;
using System.Linq;
using EvernestBack;

public class RangeCache
{
    private Queue<Tuple<EventRange, long, long>> _cachedRanges;
    private long _totalCachedRangesSize;
    private readonly long _maximumCacheSize;

    public RangeCache( long maximumCachesize)
    {
        _cachedRanges = new Queue<Tuple<EventRange, long, long>>();
        _maximumCacheSize = maximumCachesize;
    }

    public void InsertRange(EventRange range, long firstId, long lastId)
    {
        _cachedRanges.Enqueue(Tuple.Create(range, firstId, lastId));
        _totalCachedRangesSize += range.Size;
        while (_totalCachedRangesSize > _maximumCacheSize)
            _totalCachedRangesSize -= _cachedRanges.Dequeue().Item1.Size;
    }

    public bool TrySetRange(long firstId, long lastId, out EventRange range)
    {
        range = null;
        Tuple<EventRange, long, long> foundRange =
            _cachedRanges.FirstOrDefault(r => { return r.Item2 <= firstId && r.Item3 >= lastId; });
        return foundRange != null && foundRange.Item1.MakeSubRange(firstId, lastId, out range);
    }
}
