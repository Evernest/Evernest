using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract.SystemEvents
{
    class SourceRightSetSystemEvent : ISystemEvent
    {
        [DataMember]
        internal string SourceKey { get; set; }
        [DataMember]
        internal long EventStreamId { get; set; }
        [DataMember]
        internal AccessRight SourceRight { get; set; }

        [JsonConstructor]
        internal SourceRightSetSystemEvent(string sourceKey, long eventStreamId, AccessRight sourceRight)
        {
            SourceKey = sourceKey;
            EventStreamId = eventStreamId;
            SourceRight = sourceRight;
        }
    }
}
