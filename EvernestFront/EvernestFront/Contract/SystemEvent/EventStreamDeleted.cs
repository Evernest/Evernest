using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class EventStreamDeleted : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string StreamName;

        internal EventStreamDeleted(long streamId, string streamName)
        {
            StreamId = streamId;
            StreamName = streamName;
        }
    }
}
