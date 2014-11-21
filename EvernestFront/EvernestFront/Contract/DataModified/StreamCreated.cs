using System.Runtime.Serialization;

namespace EvernestFront.Contract.DataModified
{
    [DataContract]
    class StreamCreated : IDataModified
    {
        [DataMember]
        internal string StreamName;
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal long CreatorId;

        
    }
}
