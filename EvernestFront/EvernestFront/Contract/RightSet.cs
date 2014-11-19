using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class RightSet
    {
        [DataMember]
        long adminId;
        [DataMember]
        long streamId;
        [DataMember]
        long targetId;
        [DataMember]
        AccessRights right;
    }
}
