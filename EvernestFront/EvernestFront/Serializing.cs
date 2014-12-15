using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;

namespace EvernestFront
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


        internal static T ReadContract<T>(String serializedContract)
        {
            var dcs = new DataContractSerializer(typeof (T));
            var sr = new StringReader(serializedContract);
            using (var reader = XmlReader.Create(sr))
            {
                return (T) dcs.ReadObject(reader);
            }
        }

        internal static IDiff ReadDiffEnvelope(String serializedEnvelope)
        {
            DiffEnvelope envelope = ReadContract<DiffEnvelope>(serializedEnvelope);
            var diffType = envelope.DiffType;
            if (diffType == (typeof (EventStreamCreated).Name))
                return ReadContract<EventStreamCreated>(envelope.SerializedDiff);
            if (diffType == (typeof (PasswordSet)).Name)
                return ReadContract<PasswordSet>(envelope.SerializedDiff);
            if (diffType == (typeof (SourceCreated)).Name)
                return ReadContract<SourceCreated>(envelope.SerializedDiff);
            if (diffType == (typeof (UserAdded)).Name)
                return ReadContract<UserAdded>(envelope.SerializedDiff);
            if (diffType == (typeof (UserKeyCreated)).Name)
                return ReadContract<UserKeyCreated>(envelope.SerializedDiff);
            if (diffType == (typeof (UserRightSet)).Name)
                return ReadContract<UserRightSet>(envelope.SerializedDiff);
            throw new NotImplementedException();                                //should not happen. Maybe create a dummy diff that does nothing instead of throwing an exception ?


        }
    }
}
