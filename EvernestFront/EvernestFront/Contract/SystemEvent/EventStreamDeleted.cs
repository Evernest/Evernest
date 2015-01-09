using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class EventStreamDeleted : ISystemEvent
    {
        [DataMember]
        internal long StreamId;
        [DataMember]
        internal string StreamName;

        internal EventStreamDeleted(long streamId, string streamName)
        {
            StreamId = streamId;
            StreamName = streamName;
        }
    }
}
