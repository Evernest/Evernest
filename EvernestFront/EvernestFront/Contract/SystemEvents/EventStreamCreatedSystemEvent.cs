using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class EventStreamCreatedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string StreamName;
        [DataMember]
        internal string CreatorName;

        [JsonConstructor]
        internal EventStreamCreatedSystemEvent(long streamId, string streamName, string creatorName)
        {
            StreamId = streamId;
            StreamName = streamName;
            CreatorName = creatorName;
        }
    }
}
