using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class UserAdded
    {
        [DataMember]
        string UserName;
        [DataMember]
        long UserId;
        [DataMember]
        string Key; //?
    }
}
