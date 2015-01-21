using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class UserRightSetSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId { get; private set; }
        [DataMember]
        internal string TargetName { get; private set; }
        [DataMember]
        internal long TargetId { get; private set; }
        [DataMember]
        internal AccessRight Right { get; private set; }
        [DataMember]
        internal string AdminName { get; private set; }
        [DataMember]
        internal long AdminId { get; private set; }

        [JsonConstructor]
        internal UserRightSetSystemEvent(long streamId, string targetName, long targetId, AccessRight right,
            string adminName, long adminId)
        {
            StreamId = streamId;
            TargetName = targetName;
            TargetId = targetId;
            Right = right;
            AdminName = adminName;
            AdminId = adminId;
        }
    }
}
