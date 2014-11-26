
using System.Collections.Immutable;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class MakeStreamContract
    {

        static internal StreamContract NewStreamContract(string name, EvernestBack.RAMStream backStream)
        {
            var users = ImmutableDictionary<long, AccessRights>.Empty;
            return new StreamContract(name, users, backStream);
        }


        static internal StreamContract SetRight(StreamContract strmc, long streamId, AccessRights right)
        {
            var users = strmc.RelatedUsers.SetItem(streamId, right);
            return new StreamContract(strmc.StreamName, users, strmc.BackStream);
        }
    }
}
