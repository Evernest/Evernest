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
                ImmutableDictionary<long, long>.Empty
            );
        }

        internal static UserContract SetRight(UserContract usrc, long streamId, AccessRights right)
        {
            var strms = usrc.RelatedStreams.Add(streamId, right);
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