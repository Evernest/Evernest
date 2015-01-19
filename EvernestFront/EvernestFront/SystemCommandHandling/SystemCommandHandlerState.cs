using System;
using System.Collections.Generic;
using EvernestBack;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling
{
    class SystemCommandHandlerState
    {
        //TODO: initialization on system stream

        internal HashSet<string> UserNames { get; set; }
        internal Dictionary<long, UserRecord> UserIdToData { get; set; }
        internal HashSet<string> EventStreamNames { get; set; }
        internal Dictionary<long, HashSet<string>> EventStreamIdToAdmins { get; set; }

        internal long NextUserId;

        internal long NextEventStreamId;

            
        internal SystemCommandHandlerState(long numberOfREservedIds)
        {
            UserNames = new HashSet<string>();
            UserIdToData = new Dictionary<long, UserRecord>();
            EventStreamNames = new HashSet<string>();
            EventStreamIdToAdmins = new Dictionary<long, HashSet<string>>();
            // ids from 0 to numberOfREservedIds-1 are reserved for system
            NextUserId = numberOfREservedIds;
            NextEventStreamId = numberOfREservedIds;
        }





        public void Update(ISystemEvent systemEvent)
        {
            When((dynamic)systemEvent);
        }

        private void When(EventStreamCreatedSystemEvent systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
            NextEventStreamId++;
        }

        private void When(EventStreamDeletedSystemEvent systemEvent)
        {
            EventStreamNames.Remove(systemEvent.StreamName);
            EventStreamIdToAdmins.Remove(systemEvent.StreamId);
            AzureStorageClient.Instance.DeleteStreamIfExists(Convert.ToString(systemEvent.StreamId)); //TODO : maybe move this ?
        }

        private void When(PasswordSetSystemEvent systemEvent)
        {
            UserRecord userRecord;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userRecord))
                return; //TODO: report error
            userRecord.SaltedPasswordHash = systemEvent.SaltedPasswordHash;
            userRecord.PasswordSalt = systemEvent.PasswordSalt;
        }

        private void When(SourceCreatedSystemEvent systemEvent)
        {
            UserRecord userRecord;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userRecord))
                return; //TODO: report error
            userRecord.SourceNames.Add(systemEvent.SourceName);
            userRecord.SourceIdToName.Add(systemEvent.SourceId, systemEvent.SourceName);
            userRecord.NextSourceId++;
        }

        private void When(SourceDeletedSystemEvent systemEvent)
        {
            UserRecord userRecord;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userRecord))
                return; //TODO: report error
            userRecord.SourceNames.Remove(systemEvent.SourceName);
            userRecord.SourceIdToName.Remove(systemEvent.SourceId);
        }

        private void When(SourceRightSetSystemEvent systemEvent)
        {
            //nothing to do
        }

        private void When(UserCreatedSystemEvent systemEvent)
        {
            UserNames.Add(systemEvent.UserName);
            var userData = new UserRecord(systemEvent.UserName, systemEvent.SaltedPasswordHash,
                systemEvent.PasswordSalt);
            UserIdToData.Add(systemEvent.UserId, userData);
            NextUserId++;
        }

        private void When(UserDeletedSystemEvent systemEvent)
        {
            UserNames.Remove(systemEvent.UserName);
            UserIdToData.Remove(systemEvent.UserId);
        }

        private void When(UserKeyCreatedSystemEvent systemEvent)
        {
            UserRecord userRecord;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userRecord))
                return; //TODO: report error
            userRecord.KeyNames.Add(systemEvent.KeyName);
        }

        private void When(UserKeyDeletedSystemEvent systemEvent)
        {
            UserRecord userRecord;
            if (!UserIdToData.TryGetValue(systemEvent.UserId, out userRecord))
                return; //this can happen since existence is not checked in CommandToSystemEvent for key deletion
            userRecord.KeyNames.Remove(systemEvent.KeyName);
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
