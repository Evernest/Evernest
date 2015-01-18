using System.Collections.Generic;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service
{
    class ServiceData
    {
        //TODO: initialization on system stream

        internal HashSet<string> UserNames { get; set; }
        internal Dictionary<long, UserDataForService> UserIdToDatas { get; set; }
        internal HashSet<string> EventStreamNames { get; set; }
        internal Dictionary<long, HashSet<string>> EventStreamIdToAdmins { get; set; }

        private long _nextUserId;

        internal long NextUserId
        {
            get { return (_nextUserId++); }
        }

        private long _nextEventStreamId;

        internal long NextEventStreamId
        {
            get { return (_nextEventStreamId++); }
        }

            
        internal ServiceData(long numberOfREservedIds)
        {
            UserNames = new HashSet<string>();
            UserIdToDatas = new Dictionary<long, UserDataForService>();
            EventStreamNames = new HashSet<string>();
            EventStreamIdToAdmins = new Dictionary<long, HashSet<string>>();
            // ids from 0 to numberOfREservedIds-1 are reserved for system
            _nextUserId = numberOfREservedIds;
            _nextEventStreamId = numberOfREservedIds;
        }



        internal bool UserNameExists(string name)
        {
            return UserNames.Contains(name);
        }
        internal bool EventStreamNameExists(string name)
        {
            return EventStreamNames.Contains(name);
        }






        public void Update(ISystemEvent systemEvent)
        {
            When((dynamic)systemEvent);
        }

        private void When(EventStreamCreated systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
        }

        private void When(EventStreamDeleted systemEvent)
        {
            EventStreamNames.Remove(systemEvent.StreamName);
            EventStreamIdToAdmins.Remove(systemEvent.StreamId);
        }

        private void When(PasswordSet systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SaltedPasswordHash = systemEvent.SaltedPasswordHash;
            userData.PasswordSalt = systemEvent.PasswordSalt;
        }

        private void When(SourceCreated systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SourceNames.Add(systemEvent.SourceName);
            userData.SourceIdToName.Add(systemEvent.SourceId, systemEvent.SourceName);
            userData.NextSourceId++;
        }

        private void When(SourceDeleted systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SourceNames.Remove(systemEvent.SourceName);
            userData.SourceIdToName.Remove(systemEvent.SourceId);
        }

        private void When(UserCreated systemEvent)
        {
            UserNames.Add(systemEvent.UserName);
            var userData = new UserDataForService(systemEvent.UserName, systemEvent.SaltedPasswordHash,
                systemEvent.PasswordSalt);
            UserIdToDatas.Add(systemEvent.UserId, userData);
        }

        private void When(UserDeleted systemEvent)
        {
            UserNames.Remove(systemEvent.UserName);
            UserIdToDatas.Remove(systemEvent.UserId);
        }

        private void When(UserKeyCreated systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.KeyNames.Add(systemEvent.KeyName);
        }

        private void When(UserKeyDeleted systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //this can happen since existence is not checked in CommandToSystemEvent for key deletion
            userData.KeyNames.Remove(systemEvent.KeyName);
        }

        private void When(UserRightSet systemEvent)
        {
            if (systemEvent.Right == AccessRight.Admin)
            {
                HashSet<string> admins;
                if (!EventStreamIdToAdmins.TryGetValue(systemEvent.StreamId, out admins))
                    return; //TODO: report error
                admins.Add(systemEvent.TargetName);
            }
        }
    }
}
