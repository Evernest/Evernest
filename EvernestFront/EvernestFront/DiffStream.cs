using System;
using System.Runtime.Remoting.Messaging;
using EvernestBack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Answers;
using EvernestFront.Auxiliaries;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront
{
    static class DiffStream
    {

        //public long Count { get { return (int)BackStream.Index; } }
        //public long LastEventId { get { return Count - 1; } }


        internal static AzureStorageClient asc = AzureStorageClient.singleton;  //TODO : change this when AzureStorageClient becomes a singleton
        private static EvernestBack.IEventStream BackStream { get; set; }

        private const string DiffstreamStringId = "DiffStream"; //other stream IDs are made of digits, so there is no conflict. Still, probably not a good thing to hardcode this...

        static DiffStream()
        {
            BackStream = asc.GetEventStream(DiffstreamStringId);
        }

        internal static void Push(ISystemEvent systemEvent)
        {
            var contract = new SystemEventSerializationEnvelope(systemEvent);
            BackStream.Push(Serializer.WriteContract(contract), (a => { }));   //change callback to wake up projection ?
        }

        internal static ISystemEvent Pull(long id)
        {
            //TODO : check ID validity
            ISystemEvent systemEvent = null;
            BackStream.Pull(id, (a => systemEvent = Serializer.ReadDiffEnvelope(a.Message)));
            return systemEvent;
        }
    }
}
