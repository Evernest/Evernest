﻿using System.Collections.Immutable;
using EvernestFront.Contract;

namespace EvernestFront.Projections
{
    /// <summary>
    /// Data stored in SourcesProjection for each source. Immutable to allow concurrent access (unique writer, multiple readers).
    /// </summary>
    class SourceRecord
    {
        //no key field because the dictionary is indexed by them
        
        internal string SourceName { get; private set; }

        internal long SourceId { get; private set; }

        internal long UserId { get; private set; }

        internal ImmutableDictionary<long, AccessRight> RelatedEventStreams { get; set; }

        private SourceRecord(string sourceName, long sourceId, long userId,
            ImmutableDictionary<long, AccessRight> eventStreams)
        {
            SourceName = sourceName;
            SourceId = sourceId;
            UserId = userId;
            RelatedEventStreams = eventStreams;
        }

        internal SourceRecord(string sourceName, long sourceId, long userId)
            : this(sourceName, sourceId, userId, ImmutableDictionary<long, AccessRight>.Empty) { }

        internal SourceRecord SetSourceRight(long eventStreamId, AccessRight right)
        {
            var eventStreams = RelatedEventStreams.SetItem(eventStreamId, right);
            if (right == AccessRight.NoRight)
                eventStreams = eventStreams.Remove(eventStreamId);
            return new SourceRecord(SourceName, SourceId, UserId, eventStreams);
        }
    }
}
