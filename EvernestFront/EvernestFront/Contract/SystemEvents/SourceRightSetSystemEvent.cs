using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

        internal SourceRightSetSystemEvent(string sourceKey, long eventStreamId, AccessRight sourceRight)
        {
            SourceKey = sourceKey;
            EventStreamId = eventStreamId;
            SourceRight = sourceRight;
        }
    }
}
