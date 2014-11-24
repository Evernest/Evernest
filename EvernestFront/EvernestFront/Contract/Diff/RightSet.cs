using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class RightSet : IDiff
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
