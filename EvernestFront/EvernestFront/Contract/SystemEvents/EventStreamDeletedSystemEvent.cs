using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class EventStreamDeletedSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string StreamName;

        internal EventStreamDeletedSystemEvent(long streamId, string streamName)
        {
            StreamId = streamId;
            StreamName = streamName;
        }
    }
}
