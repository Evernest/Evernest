using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class StreamCreated : IDiff
    {
        
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal StreamContract StreamContract;
        [DataMember]
        internal long CreatorId;
        
        
    }
}
