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
        public String DiffType;
        
        [DataMember]
        public String SerializedDiff;

        public DiffEnvelope(IDiff diff)
        {
            DiffType = (diff.GetType()).Name;
            SerializedDiff = Serializing.WriteContract(diff);
        }
    }
}
