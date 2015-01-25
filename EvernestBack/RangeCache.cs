using System;
using System.Collections.Generic;
using System.Linq;

namespace EvernestBack
{
    /// <summary>
    /// A simple cache mechanism which stores already pulled ranges of events.
    /// </summary>
    public class RangeCache
    {
        private readonly Queue<Tuple<EventRange, long, long>> _cachedRanges;
        private long _totalCachedRangesSize;
        private readonly long _maximumCacheSize;

        /// <summary>
        /// Construct the cache.
        /// </summary>
        /// <param name="maximumCachesize">The maximum size of the cache elements (in bytes).</param>
        public RangeCache(long maximumCachesize)
        {
            _cachedRanges = new Queue<Tuple<EventRange, long, long>>();
            _maximumCacheSize = maximumCachesize;
        }

        /// <summary>
        /// Insert a range of events in the cache. May remove older ranges.
        /// </summary>
        /// <param name="range">The range to be inserted.</param>
        /// <param name="firstId">The first event's id of the range.</param>
        /// <param name="lastId">The last event's id of the range.</param>
        public void InsertRange(EventRange range, long firstId, long lastId)
        {
            _cachedRanges.Enqueue(Tuple.Create(range, firstId, lastId));
            _totalCachedRangesSize += range.Size;
            while (_totalCachedRangesSize > _maximumCacheSize)
                _totalCachedRangesSize -= _cachedRanges.Dequeue().Item1.Size;
        }

        /// <summary>
        /// Try to retrieve a range containing the specified events with the ranges stored in the cache.
        /// </summary>
        /// <param name="firstId">The first event's id.</param>
        /// <param name="lastId">The last event's id.</param>
        /// <param name="range">The range to be retrieved.</param>
        /// <returns>True if the range was successfully set, false otherwise.</returns>
        public bool TrySetRange(long firstId, long lastId, out EventRange range)
        {
            range = null;
            Tuple<EventRange, long, long> foundRange =
                _cachedRanges.FirstOrDefault(r => r.Item2 <= firstId && r.Item3 >= lastId);
            return foundRange != null && foundRange.Item1.MakeSubRange(firstId, lastId, out range);
        }
    }
}
