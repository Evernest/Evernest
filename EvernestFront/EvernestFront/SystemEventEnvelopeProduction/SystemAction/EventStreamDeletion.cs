using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.SystemEventEnvelopeProduction.SystemAction
{
    class EventStreamDeletion
    {
        internal long StreamId { get; private set; }
        internal long UserId { get; private set; }
        internal string UserPassword { get; private set; }

        internal EventStreamDeletion(long streamId, long userId, string userPassword)
        {
            StreamId = streamId;
            UserId = userId;
            UserPassword = userPassword;
        }
    }
}
