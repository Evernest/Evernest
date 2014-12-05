using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class UserAppCreated : IDiff
    {
        [DataMember]
        internal string Key { get; set; } //base64 encoded int
        [DataMember]
        internal long UserId { get; set; }

        internal UserAppCreated(string key, long userId)
        {
            Key = key;
            UserId = userId;
        }
    }
}
