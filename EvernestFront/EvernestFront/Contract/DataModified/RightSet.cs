using System.Runtime.Serialization;

namespace EvernestFront.Contract.DataModified
{
    [DataContract]
    class RightSet : IDataModified
    {
        [DataMember]
        internal long adminId;
        [DataMember]
        internal long streamId;
        [DataMember]
        internal long targetId;
        [DataMember]
        internal AccessRights right;
    }
}
