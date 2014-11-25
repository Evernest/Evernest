using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class ReadTables
    {
        internal static bool GetUserContract(Tables tbls, long userId, out UserContract userContract)
        {
            return tbls.UserTable.TryGetValue(userId, out userContract);
        }

        internal static bool GetStreamContract(Tables tbls, long streamId, out StreamContract streamContract)
        {
            return tbls.StreamTable.TryGetValue(streamId, out streamContract);
        }

        internal static bool TryGetUserId(Tables tbls, string userName, out long userId)
        {
            return tbls.UserNameToId.TryGetValue(userName, out userId);
        }

        internal static bool TryGetStreamId(Tables tbls, string streamName, out long streamId)
        {
            return tbls.StreamNameToId.TryGetValue(streamName, out streamId);
        }
    }
}
