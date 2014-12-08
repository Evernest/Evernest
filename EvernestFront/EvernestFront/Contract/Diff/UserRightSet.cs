using System.Runtime.Serialization;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class UserRightSet : IDiff
    {
        [DataMember]
        internal long AdminId;
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal long TargetId;
        [DataMember]
        internal AccessRights Right;

        internal UserRightSet(long adminId, long streamId, long targetId, AccessRights right)
        {
            AdminId = adminId;
            StreamId = streamId;
            TargetId = targetId;
            Right = right;
        }
    }
}
