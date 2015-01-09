using System.Runtime.Serialization;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class UserRightSet : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string TargetName;
        [DataMember]
        internal AccessRights Right;

        internal UserRightSet(long streamId, string targetName, AccessRights right)
        {
            StreamId = streamId;
            TargetName = targetName;
            Right = right;
        }
    }
}
