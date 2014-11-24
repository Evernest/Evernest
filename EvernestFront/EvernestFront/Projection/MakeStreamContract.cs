
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class MakeStreamContract
    {
        static internal StreamContract SetRight(StreamContract strmc, long userId, AccessRights right)
        {
            var users = strmc.RelatedUsers.Add(userId, right);
            return new StreamContract(strmc.StreamName, users, strmc.BackStream);
        }
    }
}
