using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    [DataContract]
    class UserRightSetSystemEvent : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string TargetName;
        [DataMember]
        internal AccessRight Right;

        [JsonConstructor]
        internal UserRightSetSystemEvent(long streamId, string targetName, AccessRight right)
        {
            StreamId = streamId;
            TargetName = targetName;
            Right = right;
        }
    }
}
