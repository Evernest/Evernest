using System.Collections.Immutable;
using EvernestBack;
using EvernestFront.Contract;

namespace EvernestFront.Projections
{
    /// <summary>
    /// Data stored in EventStreamProjection for each eventstream. Immutable to allow concurrent access (unique writer, multiple readers).
    /// </summary>
    class EventStreamDataForProjection
    {
        
        internal string StreamName { get; private set; }
        
        internal ImmutableDictionary<string, AccessRight> RelatedUsers { get; private set; }
        
        internal IEventStream BackStream { get; private set; }
        

        private EventStreamDataForProjection(string name, ImmutableDictionary<string, AccessRight> users, IEventStream backStream)
        {
            StreamName = name;
            RelatedUsers = users;
            BackStream = backStream;
        }

        internal EventStreamDataForProjection(string name, string creatorName, IEventStream backStream)
        {
            StreamName = name;
            RelatedUsers = ImmutableDictionary<string, AccessRight>.Empty.SetItem(creatorName, AccessRight.Admin);
            BackStream = backStream;
        }

        internal EventStreamDataForProjection SetRight(string user, AccessRight right)
        {
            var users = RelatedUsers.SetItem(user, right);
            if (right == AccessRight.NoRight)
                users = users.Remove(user);
            return new EventStreamDataForProjection(StreamName, users, BackStream);
        }
    }
}
