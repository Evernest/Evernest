﻿using System;
using System.Collections.Immutable;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    internal class UsersProjection : IProjection
    {
        private DictionariesClass Dictionaries { get; set; }

        public UsersProjection()
        {
            Dictionaries = new DictionariesClass();
        }

        public void OnSystemEvent(ISystemEvent systemEvent)
        {
            try
            {
                When((dynamic)systemEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in UsersProjection.OnSystemEvent");
                throw;
                //TODO: store errors in a stream and keep going
            }
        }

        public bool TryGetUserId(string name, out long id)
        {
            return Dictionaries.NameToId.TryGetValue(name, out id);
        }

        public bool TryGetUserData(long id, out UserDataForProjection data)
        {
            return Dictionaries.IdToData.TryGetValue(id, out data);
        }

        public bool TryGetUserIdFromKey(string key, out long id)
        {
            return Dictionaries.KeyToId.TryGetValue(key, out id);
        }

        //better than TryGetUserId followed by TryGetUserData because Dictionaries may change between the two calls
        public bool TryGetUserIdAndData(string name, out long id, out UserDataForProjection data)
        {
            var dictionaries = Dictionaries;
            if (dictionaries.NameToId.TryGetValue(name, out id))
                if (dictionaries.IdToData.TryGetValue(id, out data))
                    return true;
                else
                {
                    throw new Exception("UsersProjection.TryGetUserIdAndData");
                    //TODO: not crash
                }
            id = 0;
            data = null;
            return false;
        }

        public bool UserNameExists(string userName)
        {
            return Dictionaries.NameToId.ContainsKey(userName);
        }

        //better than TryGetUserId followed by TryGetUserData because Dictionaries may change between the two calls
        public bool TryGetUserIdAndDataByKey(string key, out long id, out UserDataForProjection data)
        {
            var dictionaries = Dictionaries;
            if (dictionaries.KeyToId.TryGetValue(key, out id))
                if (dictionaries.IdToData.TryGetValue(id, out data))
                    return true;
                else
                {
                    throw new Exception("UsersProjection.TryGetUserIdAndDataByKey");
                    //TODO: not crash
                }
            id = 0;
            data = null;
            return false;
        }

        /// <summary>
        /// Fields containing the data of the projection are encapsulated in this class so that they are atomically all set at once.
        /// </summary>
        private class DictionariesClass
        {
            internal ImmutableDictionary<string, long> NameToId { get; private set; }
            internal ImmutableDictionary<long, UserDataForProjection> IdToData { get; private set; }
            internal ImmutableDictionary<string, long> KeyToId { get; private set; } 

            private DictionariesClass(ImmutableDictionary<string, long> nti,
                ImmutableDictionary<long, UserDataForProjection> itd,
                ImmutableDictionary<string, long> kti)
            {
                NameToId = nti;
                IdToData = itd;
                KeyToId = kti;
            }

            internal DictionariesClass()
                : this(ImmutableDictionary<string, long>.Empty,
                ImmutableDictionary<long, UserDataForProjection>.Empty,
                ImmutableDictionary<string,long>.Empty) { }

            internal DictionariesClass SetNameToId(ImmutableDictionary<string, long> nti)
            {
                return new DictionariesClass(nti, IdToData, KeyToId);
            }

            internal DictionariesClass SetIdToData(ImmutableDictionary<long, UserDataForProjection> itd)
            {
                return new DictionariesClass(NameToId, itd, KeyToId);
            }

            internal DictionariesClass SetKeyToId(ImmutableDictionary<string, long> kti)
            {
                return new DictionariesClass(NameToId, IdToData, kti);
            }
        }

        

        private void SetRight(long userId, long eventStreamId, AccessRight right)
        {
            UserDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(userId, out data))
            {
                //TODO: report error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(userId, data.SetRight(eventStreamId, right));
            Dictionaries = Dictionaries.SetIdToData(itd);
        }

        private void SetRight(string userName, long eventStreamId, AccessRight right)
        {
            long userId;
            if (!Dictionaries.NameToId.TryGetValue(userName, out userId))
            {
                //this may happen: one may give rights to a nonexistent user
                return;
            }
            SetRight(userId, eventStreamId, right);
        }

        private void When(EventStreamCreated systemEvent)
        {
            SetRight(systemEvent.CreatorName, systemEvent.StreamId, AccessRight.Admin);
        }

        private void When(EventStreamDeleted systemEvent)
        {
            //delete in each user concerned?
        }

        private void When(PasswordSet systemEvent)
        {
            UserDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(systemEvent.UserId, out data))
            {
                //TODO: report error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(systemEvent.UserId,
                data.SetPassword(systemEvent.SaltedPasswordHash, systemEvent.PasswordSalt));
            Dictionaries = Dictionaries.SetIdToData(itd);
        }

        private void When(SourceCreated systemEvent)
        {
            UserDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(systemEvent.UserId, out data))
            {
                //TODO: report error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(systemEvent.UserId,
                data.AddSource(systemEvent.SourceName, systemEvent.SourceKey));
            Dictionaries = Dictionaries.SetIdToData(itd);
        }

        private void When(SourceDeleted systemEvent)
        {
            UserDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(systemEvent.UserId, out data))
            {
                //TODO: report error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(systemEvent.UserId,
                data.RemoveSource(systemEvent.SourceName));
            Dictionaries = Dictionaries.SetIdToData(itd);
        }

        

        private void When(UserCreated systemEvent)
        {
            var data = new UserDataForProjection(systemEvent.UserName,
                systemEvent.SaltedPasswordHash, systemEvent.PasswordSalt);
            var nti = Dictionaries.NameToId.SetItem(systemEvent.UserName, systemEvent.UserId);
            var itd = Dictionaries.IdToData.SetItem(systemEvent.UserId, data);
            Dictionaries = Dictionaries.SetNameToId(nti).SetIdToData(itd);
        }

        private void When(UserDeleted systemEvent)
        {
            var nti = Dictionaries.NameToId.Remove(systemEvent.UserName);
            var itd = Dictionaries.IdToData.Remove(systemEvent.UserId);
            Dictionaries = Dictionaries.SetNameToId(nti).SetIdToData(itd);
        }

        private void When(UserKeyCreated systemEvent)
        {
            UserDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(systemEvent.UserId, out data))
            {
                //TODO: report error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(systemEvent.UserId,
                data.AddUserKey(systemEvent.KeyName, systemEvent.Key));
            var kti = Dictionaries.KeyToId.SetItem(systemEvent.Key, systemEvent.UserId);
            Dictionaries = Dictionaries.SetIdToData(itd).SetKeyToId(kti);
        }

        private void When(UserKeyDeleted systemEvent)
        {
            UserDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(systemEvent.UserId, out data))
            {
                //TODO: report error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(systemEvent.UserId,
                data.RemoveUserKey(systemEvent.KeyName));
            var kti = Dictionaries.KeyToId.Remove(systemEvent.Key);
            Dictionaries = Dictionaries.SetIdToData(itd).SetKeyToId(kti);
        }
        private void When(UserRightSet systemEvent)
        {
            SetRight(systemEvent.TargetName, systemEvent.StreamId, systemEvent.Right);
        }

        
        


        
    }
}
