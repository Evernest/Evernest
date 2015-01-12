using System.Collections.Generic;
using EvernestFront.Auxiliaries;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Errors;
using EvernestFront.Service;
using EvernestFront.Service.Command;

namespace EvernestFront.Service
{
    class Service
    {
        //TODO: prevent concurrency
        //TODO: initialization on sytem stream

        private HashSet<string> UserNames { get; set; }
        private Dictionary<long, UserDataForService> UserIdToDatas { get; set; }
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

            
        internal Service()
        {
            UserNames = new HashSet<string>();
            UserIdToDatas = new Dictionary<long, UserDataForService>();
            EventStreamNames = new HashSet<string>();
            EventStreamIdToAdmins = new Dictionary<long, HashSet<string>>();
            _nextUserId = 0;
            _nextEventStreamId = 0;
        }

        internal SystemEventEnvelope ProduceSystemEventEnvelope(Command.Command action)
        {
            return ProduceSystemEventEnvelopeWhen((dynamic)action);
        }

        private bool UserNameExists(string name)
        {
            return UserNames.Contains(name);
        }
        //private bool UserIdExists(long id)
        //{
        //    return UserIdToDatas.ContainsKey(id);
        //}
        private bool EventStreamNameExists(string name)
        {
            return EventStreamNames.Contains(name);
        }
        //private bool EventStreamIdExists(long id)
        //{
        //    return EventStreamIdToAdmins.ContainsKey(id);
        //}


        private SystemEventEnvelope ProduceSystemEventEnvelopeWhen(EventStreamCreation action)
        {
            if (EventStreamNameExists(action.EventStreamName))
                return new SystemEventEnvelope(new EventStreamNameTaken(action.EventStreamName), action);
            var systemEvent = new EventStreamCreated(NextEventStreamId, action.EventStreamName, action.CreatorName);
            SelfUpdate(systemEvent);
            return new SystemEventEnvelope(systemEvent, action);
        }

        private void SelfUpdate(EventStreamCreated systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
        }

        private SystemEventEnvelope ProduceSystemEventEnvelopeWhen(EventStreamDeletion action)
        {
            HashSet<string> eventStreamAdmins;
            if (!EventStreamIdToAdmins.TryGetValue(action.EventStreamId, out eventStreamAdmins))
                return new SystemEventEnvelope(new EventStreamIdDoesNotExist(action.EventStreamId), action);
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(action.AdminId,out userData))
                return new SystemEventEnvelope(new UserIdDoesNotExist(action.AdminId), action); //TODO: change this error because it should not happen
            if (!eventStreamAdmins.Contains(userData.UserName))
                return new SystemEventEnvelope(new AdminAccessDenied(action.EventStreamId,action.AdminId), action);
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(action.AdminPassword, userData.SaltedPasswordHash, userData.PasswordSalt))
                return new SystemEventEnvelope(new WrongPassword(userData.UserName,action.AdminPassword), action);

            
            var systemEvent = new EventStreamDeleted(action.EventStreamId, action.EventStreamName);
            SelfUpdate(systemEvent);
            return new SystemEventEnvelope(systemEvent, action);
        }

        private void SelfUpdate(EventStreamDeleted systemEvent)
        {
            EventStreamNames.Remove(systemEvent.StreamName);
            EventStreamIdToAdmins.Remove(systemEvent.StreamId);
        }
    }
}
