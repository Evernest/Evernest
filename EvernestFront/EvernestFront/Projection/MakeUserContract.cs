using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class MakeUserContract
    {
        

        internal static UserContract NewUserContract(string name, string sph, byte[] ps, string key)
        {
            return new UserContract(
                name,
                sph,
                ps,
                key,
                ImmutableDictionary<long, AccessRights>.Empty,
                ImmutableDictionary<string, string>.Empty
            );
        }

        internal static UserContract AddSource(UserContract usrc, string sourceName, string sourceKey)
        {
            var srcs = usrc.OwnedSources.SetItem(sourceName, sourceKey);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash,
                usrc.PasswordSalt, usrc.Key, usrc.RelatedStreams, srcs);
        }

        internal static UserContract DeleteSource(UserContract usrc, string sourceName)
        {
            var srcs = usrc.OwnedSources.Remove(sourceName);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash,
                usrc.PasswordSalt, usrc.Key, usrc.RelatedStreams, srcs);
        }

        internal static UserContract SetRight(UserContract usrc, long streamId, AccessRights right)
        {
            var strms = usrc.RelatedStreams.SetItem(streamId, right);
            return new UserContract(usrc.UserName, usrc.SaltedPasswordHash, 
                usrc.PasswordSalt, usrc.Key, strms, usrc.OwnedSources);
        }

        internal static UserContract SetPassword(UserContract usrc, string saltedPasswordHash)
        {
            return new UserContract(usrc.UserName, saltedPasswordHash, 
                usrc.PasswordSalt, usrc.Key, usrc.RelatedStreams, usrc.OwnedSources);
        }
    }
}