using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    internal class UsersProjection : IProjection
    {
        private DictionariesClass Dictionaries { get; set; }

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

        public UsersProjection()
        {
            Dictionaries = new DictionariesClass();
        }

        public void OnSystemEvent(ISystemEvent systemEvent)
        {
            try
            {
                When((dynamic) systemEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in UsersProjection.OnSystemEvent");
                throw;
                //TODO: store errors in a stream and keep going
            }            
        }

        private void SetRight(long userId, long eventStreamId, AccessRights right)
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

        private void SetRight(string userName, long eventStreamId, AccessRights right)
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
            SetRight(systemEvent.CreatorName, systemEvent.StreamId, AccessRights.Admin);
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
