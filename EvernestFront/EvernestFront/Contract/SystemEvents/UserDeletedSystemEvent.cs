using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class UserDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal HashSet<long> RelatedEventStreams { get; private set; }
        // this set may contain event streams which do not exist anymore; it is not a problem because the projections will ignore them
         
        [JsonConstructor]
        internal UserDeletedSystemEvent(string name, long userId, HashSet<long> relatedEventStreams)
        {
            UserName = name;
            UserId = userId;
            RelatedEventStreams = relatedEventStreams;
        }
    }
}
