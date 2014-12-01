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
        internal static bool TryGetUserContract(Tables tbls, long userId, out UserContract userContract)
        {
            return tbls.UserTable.TryGetValue(userId, out userContract);
        }

        internal static bool TryGetStreamContract(Tables tbls, long streamId, out EventStreamContract streamContract)
        {
            return tbls.EventStreamTable.TryGetValue(streamId, out streamContract);
        }

        internal static bool TryGetSourceContract(Tables tbls, string sourceKey, out SourceContract sourceContract)
        {
            return tbls.SourceTable.TryGetValue(sourceKey, out sourceContract);
        }

        internal static bool TryGetUserId(Tables tbls, string userName, out long userId)
        {
            return tbls.UserNameToId.TryGetValue(userName, out userId);
        }

        internal static bool TryGetStreamId(Tables tbls, string streamName, out long streamId)
        {
            return tbls.EventStreamNameToId.TryGetValue(streamName, out streamId);
        }
    }
}
