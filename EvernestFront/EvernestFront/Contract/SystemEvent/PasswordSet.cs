using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class PasswordSet : ISystemEvent
    {
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal string SaltedPasswordHash { get; set; }

        internal PasswordSet(long userId, string saltedPasswordHash)
        {
            UserId = userId;
            SaltedPasswordHash = saltedPasswordHash;
        }
    }
}
