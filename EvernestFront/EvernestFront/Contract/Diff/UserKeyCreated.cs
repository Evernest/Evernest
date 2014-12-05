using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class UserKeyCreated : IDiff
    {
        [DataMember]
        internal string Key { get; set; } //base64 encoded int
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal string Name { get; set; }

        internal UserKeyCreated(string key, long userId, string name)
        {
            Key = key;
            UserId = userId;
            Name = name;
        }
    }
}
