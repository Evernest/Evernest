using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract.Diff
{
    [DataContract]
    class DiffEnvelope
    {
        [DataMember]
        public String diffType;
        
        [DataMember]
        public String serializedDiff;

    }
}
