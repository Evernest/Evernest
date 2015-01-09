using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Auxiliaries
{
    static class Serializing
    {
        internal static string WriteContract<T>(T contract)
        {
            var dcs = new DataContractSerializer(typeof(T));
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                dcs.WriteObject(writer, contract);
                writer.Flush();
                return sb.ToString();
            }
        }


        internal static T ReadContract<T>(string serializedContract)
        {
            var dcs = new DataContractSerializer(typeof (T));
            var sr = new StringReader(serializedContract);
            using (var reader = XmlReader.Create(sr))
            {
                return (T) dcs.ReadObject(reader);
            }
        }

        internal static ISystemEvent ReadDiffEnvelope(string serializedEnvelope)
        {
            SystemEventSerializationEnvelope serializationEnvelope = ReadContract<SystemEventSerializationEnvelope>(serializedEnvelope);
            var diffType = serializationEnvelope.SystemEventType;
            if (diffType == (typeof (EventStreamCreated).Name))
                return ReadContract<EventStreamCreated>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (PasswordSet)).Name)
                return ReadContract<PasswordSet>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (SourceCreated)).Name)
                return ReadContract<SourceCreated>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (UserCreated)).Name)
                return ReadContract<UserCreated>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (UserKeyCreated)).Name)
                return ReadContract<UserKeyCreated>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (UserRightSet)).Name)
                return ReadContract<UserRightSet>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (SourceDeleted)).Name)
                return ReadContract<SourceDeleted>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (UserKeyCreated)).Name)
                return ReadContract<UserKeyCreated>(serializationEnvelope.SerializedSystemEvent);
            if (diffType == (typeof (UserKeyDeleted)).Name)
                return ReadContract<UserKeyDeleted>(serializationEnvelope.SerializedSystemEvent);
            throw new NotImplementedException();                                //should not happen. Maybe create a dummy diff that does nothing instead of throwing an exception ?


        }
    }
}
