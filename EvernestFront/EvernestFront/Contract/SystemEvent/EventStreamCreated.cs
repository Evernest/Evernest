using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class EventStreamCreated : ISystemEvent
    {
        
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal EventStreamContract StreamContract;
        [DataMember]
        internal long CreatorId;

        internal EventStreamCreated(long streamId, EventStreamContract sc, long creatorId)
        {
            StreamId = streamId;
            StreamContract = sc;
            CreatorId = creatorId;
        }
    }
}
