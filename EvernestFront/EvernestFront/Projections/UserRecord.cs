using System.Collections.Immutable;
using EvernestFront.Contract;

namespace EvernestFront.Projections
{
    class UserRecord
    {
        internal string UserName { get; set; }
       
        internal string SaltedPasswordHash { get; set; }
     
        internal byte[] PasswordSalt { get; set; }

        internal ImmutableDictionary<string, string> Keys { get; set; }

        internal ImmutableHashSet<long> RelatedEventStreams { get; set; }
    
        internal ImmutableDictionary<string, long> SourceNameToId { get; set; }

        internal ImmutableDictionary<long, string> SourceIdToKey { get; set; }

        private UserRecord(string name, string hash, byte[] salt,
            ImmutableDictionary<string, string> keys, ImmutableHashSet<long> eventStreams,
            ImmutableDictionary<string, long> sources, ImmutableDictionary<long, string> sourceKeys)
        {
            UserName = name;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
            Keys = keys;
            RelatedEventStreams = eventStreams;
            SourceNameToId = sources;
            SourceIdToKey = sourceKeys;
        }

        internal UserRecord(string name, string hash, byte[] salt)
            : this(name, hash, salt, ImmutableDictionary<string, string>.Empty,
                ImmutableHashSet<long>.Empty, ImmutableDictionary<string, long>.Empty,
                ImmutableDictionary<long, string>.Empty) { }

        internal UserRecord SetPassword(string hash, byte[] salt)
        {
            return new UserRecord(UserName, hash, salt, Keys, RelatedEventStreams, SourceNameToId, SourceIdToKey);
        }

        internal UserRecord SetUserRight(long eventStream, AccessRight right)
        {
            ImmutableHashSet<long> eventStreams;
            if (right == AccessRight.NoRight)
                eventStreams = RelatedEventStreams.Remove(eventStream);
            else
                eventStreams = RelatedEventStreams.Add(eventStream);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, eventStreams,
                SourceNameToId, SourceIdToKey);
        }

        internal UserRecord RemoveEventStream(long id)
        {
            var eventStreams = RelatedEventStreams.Remove(id);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, eventStreams,
                SourceNameToId, SourceIdToKey);
        }

        internal UserRecord AddSource(string name, long id, string key)
        {
            var sources = SourceNameToId.SetItem(name, id);
            var sourceKeys = SourceIdToKey.SetItem(id, key);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, RelatedEventStreams,
                sources, sourceKeys);
        }

        internal UserRecord RemoveSource(string name, long id)
        {
            var sources = SourceNameToId.Remove(name);
            var sourceKeys = SourceIdToKey.Remove(id);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, RelatedEventStreams,
                sources, sourceKeys);
        }

        internal UserRecord AddUserKey(string name, string key)
        {
            var keys = Keys.SetItem(name, key);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, keys, RelatedEventStreams,
                SourceNameToId, SourceIdToKey);
        }

        internal UserRecord RemoveUserKey(string name)
        {
            var keys = Keys.Remove(name);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, keys, RelatedEventStreams,
                SourceNameToId, SourceIdToKey);
        }
    }
}
