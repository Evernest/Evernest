using System;
using EvernestBack;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront
{
    class SystemEventStream
    {
        private readonly IEventStream _backEventStream;

        public SystemEventStream(long systemEventStreamId)
        {
            //TODO: update when method TryGet... is implemented in AzureStorageClient
            var stringId = Convert.ToString(systemEventStreamId);
            var azureStorageClient = AzureStorageClient.Instance;
            try
            {
                _backEventStream = azureStorageClient.GetNewEventStream(stringId);
            }
            catch (ArgumentException)
            {
                _backEventStream = azureStorageClient.GetEventStream(stringId);
            }
        }

        public void CreateSystemStream()
        {

        }

        internal void Push(ISystemEvent systemEvent)
        {
            var serializer = new Serializer();
            var contract = new SystemEventEnvelope(systemEvent);
            var contractString = serializer.WriteContract(contract);
            _backEventStream.Push(contractString, CallbackSuccess, CallbackFailure);
        }
        private void CallbackSuccess(IAgent agent) { }
        private void CallbackFailure(IAgent agent, string s) { }

        //TODO: reading in system event stream
    }


}
