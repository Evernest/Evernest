using System.Collections.Immutable;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class MakeUserContract
    {
        

        internal static UserContract NewUser(string name, string sph, byte[] ps)
        {
            return new UserContract(
                name,
                sph,
                ps,
                ImmutableDictionary<string, string>.Empty,
                ImmutableDictionary<long, AccessRights>.Empty,
                ImmutableDictionary<string, string>.Empty
            );
        }

        internal static UserContract AddKey(UserContract usrc, string keyName, string key)
        {
            var keys = usrc.Keys.SetItem(keyName, key);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash,
                usrc.PasswordSalt, keys, usrc.RelatedStreams, usrc.OwnedSources);
        }
        internal static UserContract RemoveKey(UserContract usrc, string keyName)
        {
            var keys = usrc.Keys.Remove(keyName);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash,
                usrc.PasswordSalt, keys, usrc.RelatedStreams, usrc.OwnedSources);
        }

        internal static UserContract AddSource(UserContract usrc, string sourceName, string sourceKey)
        {
            var srcs = usrc.OwnedSources.SetItem(sourceName, sourceKey);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash,
                usrc.PasswordSalt, usrc.Keys, usrc.RelatedStreams, srcs);
        }

        internal static UserContract RemoveSource(UserContract usrc, string sourceName)
        {
            var srcs = usrc.OwnedSources.Remove(sourceName);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash,
                usrc.PasswordSalt, usrc.Keys, usrc.RelatedStreams, srcs);
        }

        internal static UserContract SetRight(UserContract usrc, long streamId, AccessRights right)
        {
            var strms = usrc.RelatedStreams.SetItem(streamId, right);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash, 
                usrc.PasswordSalt, usrc.Keys, strms, usrc.OwnedSources);
        }

        internal static UserContract SetPassword(UserContract usrc, string saltedPasswordHash)
        {
            return new UserContract(usrc.UserName, saltedPasswordHash, 
                usrc.PasswordSalt, usrc.Keys, usrc.RelatedStreams, usrc.OwnedSources);
        }
    }
}