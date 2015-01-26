using System.Collections.Generic;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Projections;

namespace EvernestFront.SystemCommandHandling
{
    /// <summary>
    /// This is the state of the SystemCommandHandler, which uses it to validate commands. 
    /// It implements IProjection because it is updated through system events. 
    /// When SystemCommandHandler produces a system events, this is updated before handling
    /// the next system command, so it is always totally accurate.
    /// This is also updated at startup on all the system events read on the system
    /// history stream.
    /// </summary>
    class SystemCommandHandlerState : IProjection
    {
        //TODO: initialization on system stream

        internal Dictionary<string, long> UserNameToId { get; set; }
        internal Dictionary<long, UserRecord> UserIdToData { get; set; }
        internal HashSet<string> EventStreamNames { get; set; }
        internal Dictionary<long, HashSet<string>> EventStreamIdToAdmins { get; set; }
        internal Dictionary<long, HashSet<long>> EventStreamIdToUsers { get; set; } //used during eventstream deletion, to remove it from each user
        
        internal long NextUserId;

        internal long NextEventStreamId;

            
        internal SystemCommandHandlerState(long numberOfREservedIds)
        {
            UserNameToId = new Dictionary<string, long>();
            UserIdToData = new Dictionary<long, UserRecord>();
            EventStreamNames = new HashSet<string>();
            EventStreamIdToAdmins = new Dictionary<long, HashSet<string>>();
            EventStreamIdToUsers = new Dictionary<long, HashSet<long>>();
            // ids from 0 to numberOfREservedIds-1 are reserved for system
            NextUserId = numberOfREservedIds;
            NextEventStreamId = numberOfREservedIds;
        }





        public void OnSystemEvent(ISystemEvent systemEvent)
        {
            When((dynamic)systemEvent);
        }

        private void When(EventStreamCreatedSystemEvent systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
            EventStreamIdToUsers.Add(systemEvent.StreamId, new HashSet<long> { systemEvent.CreatorId });
            NextEventStreamId++;
            UserRecord userRecord;
            if (UserIdToData.TryGetValue(systemEvent.CreatorId, out userRecord))
                userRecord.RelatedEventStreams.Add(systemEvent.StreamId);
        }

        private void When(EventStreamDeletedSystemEvent systemEvent)
        {
            EventStreamNames.Remove(systemEvent.StreamName);
            EventStreamIdToAdmins.Remove(systemEvent.StreamId);
            EventStreamIdToUsers.Remove(systemEvent.StreamId);
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
            UserNameToId.Add(systemEvent.UserName, systemEvent.UserId);
            var userData = new UserRecord(systemEvent.UserName, systemEvent.SaltedPasswordHash,
                systemEvent.PasswordSalt);
            UserIdToData.Add(systemEvent.UserId, userData);
            NextUserId++;
        }

        private void When(UserDeletedSystemEvent systemEvent)
        {
            UserNameToId.Remove(systemEvent.UserName);
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
                if (EventStreamIdToAdmins.TryGetValue(systemEvent.StreamId, out admins))
                    admins.Add(systemEvent.TargetName);
            }
            HashSet<long> users;
            if (EventStreamIdToUsers.TryGetValue(systemEvent.StreamId, out users))
            {
                if (systemEvent.Right == AccessRight.NoRight)
                    users.Remove(systemEvent.TargetId);
                else
                    users.Add(systemEvent.TargetId);
            }
            UserRecord targetUserRecord;
            if (UserIdToData.TryGetValue(systemEvent.TargetId, out targetUserRecord))
            {
                if (systemEvent.Right == AccessRight.NoRight)
                    targetUserRecord.RelatedEventStreams.Remove(systemEvent.StreamId);
                else
                    targetUserRecord.RelatedEventStreams.Add(systemEvent.StreamId);
            }
        }
    }
}
