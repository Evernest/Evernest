using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class StreamCreated
    {
        [DataMember]
        string StreamName;
        [DataMember]
        long StreamId;
        [DataMember]
        long CreatorId;
        [DataMember]
    }
}
