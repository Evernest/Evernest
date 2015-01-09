using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Errors;
using EvernestFront.SystemEventEnvelopeProduction.SystemAction;

namespace EvernestFront.SystemEventEnvelopeProduction
{
    class SystemEventEnvelopeProducer
    {
        //TODO: prevent concurrency
        //TODO: initialization on sytem stream

        private HashSet<string> UserNames { get; set; }
        private Dictionary<long, UserDataForProducer> UserIdToDatas { get; set; }
        private HashSet<string> EventStreamNames { get; set; }
        private Dictionary<long, HashSet<string>> EventStreamIdToAdmins { get; set; }

        private long _nextUserId;

        private long NextUserId
        {
            get { return (_nextUserId++); }
        }

        private long _nextEventStreamId;

        private long NextEventStreamId
        {
            get { return (_nextEventStreamId++); }
        }

            
        internal SystemEventEnvelopeProducer()
        {
            UserNames = new HashSet<string>();
            UserIdToDatas = new Dictionary<long, UserDataForProducer>();
            EventStreamNames = new HashSet<string>();
            EventStreamIdToAdmins = new Dictionary<long, HashSet<string>>();
            _nextUserId = 0;
            _nextEventStreamId = 0;
        }

        internal SystemEventEnvelope ProduceSystemEventEnvelope(SystemAction.SystemAction action)
        {
            return ProduceSystemEventEnvelopeWhen((dynamic)action);
        }

        private bool UserNameExists(string name)
        {
            return UserNames.Contains(name);
        }
        private bool EventStreamNameExists(string name)
        {
            return EventStreamNames.Contains(name);
        }


        private SystemEventEnvelope ProduceSystemEventEnvelopeWhen(EventStreamCreation action)
        {
            if (EventStreamNameExists(action.StreamName))
                return new SystemEventEnvelope(new EventStreamNameTaken(action.StreamName), action);
            var systemEvent = new EventStreamCreated(NextEventStreamId, action.StreamName, action.CreatorName);
            SelfUpdate(systemEvent);
            return new SystemEventEnvelope(systemEvent, action);
        }

        private void SelfUpdate(EventStreamCreated systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
        }
    }
}
