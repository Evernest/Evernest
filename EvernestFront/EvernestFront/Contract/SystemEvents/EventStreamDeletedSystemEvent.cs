using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class EventStreamDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId { get; private set; }
        [DataMember]
        internal string StreamName { get; private set; }
        [DataMember]
        internal long AdminId { get; private set; }
        [DataMember]
        internal string AdminName { get; private set; }
        [DataMember]
        internal HashSet<long> RelatedUsers { get; private set; }
        // this set may contain users which do not exist anymore; it is not a problem because the projections will ignore them

        [JsonConstructor]
        internal EventStreamDeletedSystemEvent(long streamId, string streamName, 
            string adminName, long adminId, HashSet<long> relatedUsers)
        {
            StreamId = streamId;
            StreamName = streamName;
            AdminName = adminName;
            AdminId = adminId;
            RelatedUsers = relatedUsers;
        }
    }
}
