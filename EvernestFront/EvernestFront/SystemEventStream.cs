using System.Collections.Immutable;
using EvernestBack;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront
{
    class SystemEventStreamManager
    {
        //TODO: implement this class
        //private readonly string _systemUserName = "RootUser";
        //private readonly SystemUser _systemUser;
        //private EventStream _systemStream;

        public void CreateSystemStream()
        {

        }

        internal void Push(ISystemEvent systemEvent)
        {
            //var serializer = new Serializer();
            //var contract = new SystemEventEnvelope(systemEvent);
            //_systemStream.Push(serializer.WriteContract(contract));
        }
    }


}
