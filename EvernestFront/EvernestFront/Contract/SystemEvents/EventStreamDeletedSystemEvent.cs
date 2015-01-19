using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class EventStreamDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string StreamName;

        [JsonConstructor]
        internal EventStreamDeletedSystemEvent(long streamId, string streamName)
        {
            StreamId = streamId;
            StreamName = streamName;
        }
    }
}
