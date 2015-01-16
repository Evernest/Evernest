using System.Collections.Immutable;

namespace EvernestFront.Projections
{
    /// <summary>
    /// Data stored in EventStreamProjection for each eventstream. Immutable to allow concurrent access (unique writer, multiple readers).
    /// </summary>
    class EventStreamDataForProjection
    {
        
        internal string StreamName { get; private set; }
        
        internal ImmutableDictionary<string, AccessRights> RelatedUsers { get; private set; }
        
        internal EvernestBack.RAMStream BackStream { get; private set; }
        //this is a temporary simulator of backend
        

        private EventStreamDataForProjection(string name, ImmutableDictionary<string, AccessRights> users, EvernestBack.RAMStream bs)
        {
            StreamName = name;
            RelatedUsers = users;
            BackStream = bs;
        }

        internal EventStreamDataForProjection(string name, string creatorName, EvernestBack.RAMStream backStream)
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
