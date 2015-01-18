using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class EventStreamCreated : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string StreamName;
        [DataMember]
        internal string CreatorName;

        internal EventStreamCreated(long streamId, string streamName, string creatorName)
        {
            StreamId = streamId;
            StreamName = streamName;
            CreatorName = creatorName;
        }
    }
}
