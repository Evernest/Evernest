using System.Collections.Immutable;

namespace EvernestFront.Projections
{
    class SourceDataForProjection
    {
        //no key field because the dictionary is indexed by them
        
        internal string SourceName { get; private set; }

        internal long SourceId { get; private set; }

        internal long UserId { get; private set; }

        internal ImmutableDictionary<long, AccessRight> RelatedEventStreams { get; set; }

        private SourceDataForProjection(string sourceName, long sourceId, long userId,
            ImmutableDictionary<long, AccessRight> eventStreams)
        {
            SourceName = sourceName;
            SourceId = sourceId;
            UserId = userId;
            RelatedEventStreams = eventStreams;
        }

        internal SourceDataForProjection(string sourceName, long sourceId, long userId)
            : this(sourceName, sourceId, userId, ImmutableDictionary<long, AccessRight>.Empty) { }
    }
}
