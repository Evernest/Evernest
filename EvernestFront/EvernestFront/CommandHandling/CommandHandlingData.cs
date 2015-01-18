using System.Collections.Generic;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling
{
    class CommandHandlingData
    {
        //TODO: initialization on system stream

        internal HashSet<string> UserNames { get; set; }
        internal Dictionary<long, CommandHandlingUserData> UserIdToData { get; set; }
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

            
        internal CommandHandlingData(long numberOfREservedIds)
        {
            UserNames = new HashSet<string>();
            UserIdToData = new Dictionary<long, CommandHandlingUserData>();
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

        private void When(EventStreamCreatedSystemEvent systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
        }

        private void When(EventStreamDeletedSystemEvent systemEvent)
        {
            EventStreamNames.Remove(systemEvent.StreamName);
            EventStreamIdToAdmins.Remove(systemEvent.StreamId);
        }

        private void When(PasswordSetSystemEvent systemEvent)
        {
            CommandHandlingUserData userData;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SaltedPasswordHash = systemEvent.SaltedPasswordHash;
            userData.PasswordSalt = systemEvent.PasswordSalt;
        }

        private void When(SourceCreatedSystemEvent systemEvent)
        {
            CommandHandlingUserData userData;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SourceNames.Add(systemEvent.SourceName);
            userData.SourceIdToName.Add(systemEvent.SourceId, systemEvent.SourceName);
            userData.NextSourceId++;
        }

        private void When(SourceDeletedSystemEvent systemEvent)
        {
            CommandHandlingUserData userData;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SourceNames.Remove(systemEvent.SourceName);
            userData.SourceIdToName.Remove(systemEvent.SourceId);
        }

        private void When(SourceRightSetSystemEvent systemEvent)
        {
            //nothing to do
        }

        private void When(UserCreatedSystemEvent systemEvent)
        {
            UserNames.Add(systemEvent.UserName);
            var userData = new CommandHandlingUserData(systemEvent.UserName, systemEvent.SaltedPasswordHash,
                systemEvent.PasswordSalt);
            UserIdToData.Add(systemEvent.UserId, userData);
        }

        private void When(UserDeletedSystemEvent systemEvent)
        {
            UserNames.Remove(systemEvent.UserName);
            UserIdToData.Remove(systemEvent.UserId);
        }

        private void When(UserKeyCreatedSystemEvent systemEvent)
        {
            CommandHandlingUserData userData;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.KeyNames.Add(systemEvent.KeyName);
        }

        private void When(UserKeyDeletedSystemEvent systemEvent)
        {
            CommandHandlingUserData userData;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userData))
                return; //this can happen since existence is not checked in CommandToSystemEvent for key deletion
            userData.KeyNames.Remove(systemEvent.KeyName);
        }

        private void When(UserRightSetSystemEvent systemEvent)
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
