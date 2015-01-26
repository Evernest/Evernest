using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EvernestBack;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront
{
    /// <summary>
    /// Used to store all past SystemEvents on Azure. 
    /// After a shutdown, they can be retrieved using the method PullAll and used by a SystemEventRecuperator to rebuild the pre-shutdown system state.
    /// </summary>
    class SystemEventStream
    {
        private readonly IEventStream _backEventStream;

        public SystemEventStream(AzureStorageClient azureStorageClient, long systemEventStreamId)
        {
            if(!azureStorageClient.TryGetFreshEventStream(systemEventStreamId, out _backEventStream))
                _backEventStream = azureStorageClient.GetEventStream(systemEventStreamId);
        }

        internal void Push(ISystemEvent systemEvent)
        {
            var serializer = new Serializer();
            var contract = new SystemEventEnvelope(systemEvent.GetType().Name,serializer.WriteContract(systemEvent));
            var contractString = serializer.WriteContract(contract);
            _backEventStream.Push(contractString, PushCallbackSuccess, PushCallbackFailure);
        }
        private void PushCallbackSuccess(LowLevelEvent acceptedEvent) { }
        private void PushCallbackFailure(string deniedQuery, string errorMessage) { }

        internal List<ISystemEvent> PullAll()
        {
            var eventList = new List<ISystemEvent>();
            var stopWaitHandle = new AutoResetEvent(false);
            var serializer = new Serializer();
            var size = _backEventStream.Size();
            _backEventStream.PullRange
            (
                0,
                size-1,
                range =>
                {
                    eventList.AddRange(range.Select(ev => serializer.ReadSystemEventEnvelope(ev.Message)));
                    stopWaitHandle.Set();
                },
                (firstId, lastId, errorMessage) => stopWaitHandle.Set());
            stopWaitHandle.WaitOne();
            return eventList;
        }

    }



}
