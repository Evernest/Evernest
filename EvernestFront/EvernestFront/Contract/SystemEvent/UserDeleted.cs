using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class UserDeleted
    {
        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal long UserId { get; set; }

        internal UserDeleted(string name, long userId)
        {
            UserName = name;
            UserId = userId;
        }
    }
}
