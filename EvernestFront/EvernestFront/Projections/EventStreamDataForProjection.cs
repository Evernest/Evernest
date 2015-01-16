using System.Collections.Immutable;
using EvernestBack;

namespace EvernestFront.Projections
{
    /// <summary>
    /// Data stored in EventStreamProjection for each eventstream. Immutable to allow concurrent access (unique writer, multiple readers).
    /// </summary>
    class EventStreamDataForProjection
    {
        
        internal string StreamName { get; private set; }
        
        internal ImmutableDictionary<string, AccessRights> RelatedUsers { get; private set; }
        
        internal EvernestBack.IEventStream BackStream { get; private set; }
        

        private EventStreamDataForProjection(string name, ImmutableDictionary<string, AccessRights> users, IEventStream backStream)
        {
            StreamName = name;
            RelatedUsers = users;
            BackStream = backStream;
        }

        internal EventStreamDataForProjection(string name, string creatorName, IEventStream backStream)
        {
            StreamName = name;
            RelatedUsers = ImmutableDictionary<string, AccessRights>.Empty.SetItem(creatorName, AccessRights.Admin);
            BackStream = backStream;
        }

        internal EventStreamDataForProjection SetRight(string user, AccessRights right)
        {
            var users = RelatedUsers.SetItem(user, right);
            return new EventStreamDataForProjection(StreamName, users, BackStream);
        }
    }
}
