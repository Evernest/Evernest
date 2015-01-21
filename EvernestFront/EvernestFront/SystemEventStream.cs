﻿using System;
using EvernestBack;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront
{
    class SystemEventStream
    {
        private readonly IEventStream _backEventStream;

        public SystemEventStream(AzureStorageClient azureStorageClient, long systemEventStreamId)
        {
            //if (!azureStorageClient.TryGetFreshEventStream(systemEventStreamId, out _backEventStream))
            //    _backEventStream = azureStorageClient.GetEventStream(systemEventStreamId);
            //TODO: update when method TryGet... is implemented in AzureStorageClient
            try
            {
                _backEventStream = azureStorageClient.GetNewEventStream(systemEventStreamId);
            }
            catch (ArgumentException)
            {
                _backEventStream = azureStorageClient.GetEventStream(systemEventStreamId);
            }
        }

        public void CreateSystemStream()
        {

        }

        internal void Push(ISystemEvent systemEvent)
        {
            var serializer = new Serializer();
            var contract = new SystemEventEnvelope(systemEvent.GetType().Name,serializer.WriteContract(systemEvent));
            var contractString = serializer.WriteContract(contract);
            _backEventStream.Push(contractString, CallbackSuccess, CallbackFailure);
        }
        private void CallbackSuccess(LowLevelEvent acceptedEvent) { }
        private void CallbackFailure(string deniedQuery, string errorMessage) { }

        //TODO: reading in system event stream
    }


}
