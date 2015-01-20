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

        internal ImmutableDictionary<long, AccessRight> RelatedEventStreams { get; set; }
    
        internal ImmutableDictionary<string, long> Sources { get; set; } //name->id

        internal ImmutableDictionary<long, string> SourceKeys { get; set; } //id->key

        private UserRecord(string name, string hash, byte[] salt,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<long, AccessRight> eventStreams,
            ImmutableDictionary<string, long> sources, ImmutableDictionary<long, string> sourceKeys)
        {
            UserName = name;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
            Keys = keys;
            RelatedEventStreams = eventStreams;
            Sources = sources;
            SourceKeys = sourceKeys;
        }

        internal UserRecord(string name, string hash, byte[] salt)
            : this(name, hash, salt, ImmutableDictionary<string, string>.Empty,
                ImmutableDictionary<long, AccessRight>.Empty, ImmutableDictionary<string, long>.Empty,
                ImmutableDictionary<long, string>.Empty) { }

        internal UserRecord SetPassword(string hash, byte[] salt)
        {
            return new UserRecord(UserName, hash, salt, Keys, RelatedEventStreams, Sources, SourceKeys);
        }

        internal UserRecord SetUserRight(long eventStream, AccessRight right)
        {
            var eventStreams = RelatedEventStreams.SetItem(eventStream, right);
            if (right == AccessRight.NoRight)
                eventStreams = eventStreams.Remove(eventStream);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, eventStreams,
                Sources, SourceKeys);
        }

        internal UserRecord AddSource(string name, long id, string key)
        {
            var sources = Sources.SetItem(name, id);
            var sourceKeys = SourceKeys.SetItem(id, key);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, RelatedEventStreams,
                sources, sourceKeys);
        }

        internal UserRecord RemoveSource(string name, long id)
        {
            var sources = Sources.Remove(name);
            var sourceKeys = SourceKeys.Remove(id);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, Keys, RelatedEventStreams,
                sources, sourceKeys);
        }

        internal UserRecord AddUserKey(string name, string key)
        {
            var keys = Keys.SetItem(name, key);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, keys, RelatedEventStreams,
                Sources, SourceKeys);
        }

        internal UserRecord RemoveUserKey(string name)
        {
            var keys = Keys.Remove(name);
            return new UserRecord(UserName, SaltedPasswordHash, PasswordSalt, keys, RelatedEventStreams,
                Sources, SourceKeys);
        }
    }
}
