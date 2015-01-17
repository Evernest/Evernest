﻿using System.Collections.Generic;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service
{
    class ServiceData
    {
        //TODO: prevent concurrency
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

            
        internal ServiceData()
        {
            UserNames = new HashSet<string>();
            UserIdToDatas = new Dictionary<long, UserDataForService>();
            EventStreamNames = new HashSet<string>();
            EventStreamIdToAdmins = new Dictionary<long, HashSet<string>>();
            _nextUserId = 0;
            _nextEventStreamId = 0;
        }



        internal bool UserNameExists(string name)
        {
            return UserNames.Contains(name);
        }
        //internal bool UserIdExists(long id)
        //{
        //    return UserIdToDatas.ContainsKey(id);
        //}
        internal bool EventStreamNameExists(string name)
        {
            return EventStreamNames.Contains(name);
        }
        //internal bool EventStreamIdExists(long id)
        //{
        //    return EventStreamIdToAdmins.ContainsKey(id);
        //}






        internal void SelfUpdate(ISystemEvent systemEvent)
        {
            SelfUpdateCase((dynamic)systemEvent);
        }

        private void SelfUpdateCase(EventStreamCreated systemEvent)
        {
            EventStreamNames.Add(systemEvent.StreamName);
            EventStreamIdToAdmins.Add(systemEvent.StreamId, new HashSet<string> { systemEvent.CreatorName });
        }

        private void SelfUpdateCase(EventStreamDeleted systemEvent)
        {
            EventStreamNames.Remove(systemEvent.StreamName);
            EventStreamIdToAdmins.Remove(systemEvent.StreamId);
        }

        private void SelfUpdateCase(PasswordSet systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.SaltedPasswordHash = systemEvent.SaltedPasswordHash;
            userData.PasswordSalt = systemEvent.PasswordSalt;
        }

        private void SelfUpdateCase(UserCreated systemEvent)
        {
            UserNames.Add(systemEvent.UserName);
            var userData = new UserDataForService(systemEvent.UserName, systemEvent.SaltedPasswordHash,
                systemEvent.PasswordSalt);
            UserIdToDatas.Add(systemEvent.UserId, userData);
        }

        private void SelfUpdateCase(UserDeleted systemEvent)
        {
            UserNames.Remove(systemEvent.UserName);
            UserIdToDatas.Remove(systemEvent.UserId);
        }

        private void SelfUpdateCase(UserKeyCreated systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //TODO: report error
            userData.Keys.Add(systemEvent.KeyName);
        }

        private void SelfUpdateCase(UserKeyDeleted systemEvent)
        {
            UserDataForService userData;
            if (!UserIdToDatas.TryGetValue(systemEvent.UserId, out userData))
                return; //this can happen since existence is not checked in CommandToSystemEvent for key deletion
            userData.Keys.Remove(systemEvent.KeyName);
        }

        private void SelfUpdateCase(UserRightSet systemEvent)
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
