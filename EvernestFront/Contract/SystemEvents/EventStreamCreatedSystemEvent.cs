using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class EventStreamCreatedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId { get; private set; }
        [DataMember]
        internal string StreamName { get; private set; }
        [DataMember]
        internal string CreatorName { get; private set; }
        [DataMember]
        internal long CreatorId { get; private set; }

        [JsonConstructor]
        internal EventStreamCreatedSystemEvent(long streamId, string streamName, string creatorName, long creatorId)
        {
            StreamId = streamId;
            StreamName = streamName;
            CreatorName = creatorName;
            CreatorId = creatorId;
        }
    }
}
