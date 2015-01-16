using System.Collections.Immutable;

namespace EvernestFront.Projections
{
    class UserDataForProjection
    {
        internal string UserName { get; set; }
       
        internal string SaltedPasswordHash { get; set; }
     
        internal byte[] PasswordSalt { get; set; }

        internal ImmutableDictionary<string, string> Keys { get; set; } //base64 encoded int

        internal ImmutableDictionary<long, AccessRights> RelatedEventStreams { get; set; }
    
        internal ImmutableDictionary<string, string> Sources { get; set; } //name->key

        private UserDataForProjection(string name, string hash, byte[] salt,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<long, AccessRights> eventStreams,
            ImmutableDictionary<string, string> sources)
        {
            UserName = name;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
            Keys = keys;
            RelatedEventStreams = eventStreams;
            Sources = sources;
        }

        internal UserDataForProjection(string name, string hash, byte[] salt)
            : this(name, hash, salt, ImmutableDictionary<string, string>.Empty,
                ImmutableDictionary<long, AccessRights>.Empty, ImmutableDictionary<string, string>.Empty) { }

        internal UserDataForProjection SetPassword(string hash, byte[] salt)
        {
            return new UserDataForProjection(UserName, hash, salt, Keys, RelatedEventStreams, Sources);
        }

        internal UserDataForProjection SetRight(long eventStream, AccessRights right)
        {
            var eventStreams = RelatedEventStreams.SetItem(eventStream, right);
            return new UserDataForProjection(UserName, SaltedPasswordHash, PasswordSalt, Keys, eventStreams,
                Sources);
        }

        internal UserDataForProjection AddSource(string name, string key)
        {
            var sources = Sources.SetItem(name, key);
            return new UserDataForProjection(UserName, SaltedPasswordHash, PasswordSalt, Keys, RelatedEventStreams,
                sources);
        }

        internal UserDataForProjection RemoveSource(string name)
        {
            var sources = Sources.Remove(name);
            return new UserDataForProjection(UserName, SaltedPasswordHash, PasswordSalt, Keys, RelatedEventStreams,
                sources);
        }

        internal UserDataForProjection AddUserKey(string name, string key)
        {
            var keys = Keys.SetItem(name, key);
            return new UserDataForProjection(UserName, SaltedPasswordHash, PasswordSalt, keys, RelatedEventStreams,
                Sources);
        }

        internal UserDataForProjection RemoveUserKey(string name)
        {
            var keys = Keys.Remove(name);
            return new UserDataForProjection(UserName, SaltedPasswordHash, PasswordSalt, keys, RelatedEventStreams,
                Sources);
        }
    }
}
