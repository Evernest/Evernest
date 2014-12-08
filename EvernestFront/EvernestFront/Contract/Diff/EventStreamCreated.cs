using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class EventStreamCreated : IDiff
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
