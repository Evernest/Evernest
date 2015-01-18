﻿using System.Runtime.Serialization;
using EvernestFront.Utilities;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.Contract
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
            var serializer = new Serializer();
            SystemEventType = (systemEvent.GetType()).Name;
            SerializedSystemEvent = serializer.WriteContract(systemEvent);
        }
    }
}