
using System.Collections.Immutable;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class MakeEventStreamContract
    {

        static internal EventStreamContract NewStreamContract(string name, EvernestBack.RAMStream backStream)
        {
            var users = ImmutableDictionary<long, AccessRights>.Empty;
            return new EventStreamContract(name, users, backStream);
        }


        static internal EventStreamContract SetRight(EventStreamContract strmc, long streamId, AccessRights right)
        {
            var users = strmc.RelatedUsers.SetItem(streamId, right);
            return new EventStreamContract(strmc.StreamName, users, strmc.BackStream);
        }
    }
}
