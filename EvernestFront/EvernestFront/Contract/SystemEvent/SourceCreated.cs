using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class SourceCreated : ISystemEvent
    {
        [DataMember]
        internal string Key { get; set; } //base64 encoded int
        [DataMember]
        internal SourceContract SourceContract { get; set; }

        internal SourceCreated(string key, SourceContract sc)
        {
            Key = key;
            SourceContract = sc;
        }
    }
}
