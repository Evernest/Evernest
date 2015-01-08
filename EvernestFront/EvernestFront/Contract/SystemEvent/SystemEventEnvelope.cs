using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Auxiliaries;

namespace EvernestFront.Contract.SystemEvent
{
    [DataContract]
    class SystemEventEnvelope
    {
        [DataMember]
        public string SystemEventType;
        
        [DataMember]
        public string SerializedSystemEvent;

        public SystemEventEnvelope(ISystemEvent systemEvent)
        {
            SystemEventType = (systemEvent.GetType()).Name;
            SerializedSystemEvent = Serializing.WriteContract(systemEvent);
        }
    }
}
