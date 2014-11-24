using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class StreamCreated : IDiff
    {
        [DataMember]
        internal string StreamName;
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal long CreatorId;

        
    }
}
