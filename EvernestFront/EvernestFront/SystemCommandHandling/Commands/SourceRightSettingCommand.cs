using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class SourceRightSettingCommand : CommandBase
    {
        internal readonly long SourceId;

        internal readonly long UserId;

        internal readonly string SourceKey;

        internal readonly long EventStreamId;

        internal readonly AccessRight SourceRight;

        internal SourceRightSettingCommand(SystemCommandHandler systemCommandHandler, long userId, long sourceId, 
            string sourceKey, long eventStreamId, AccessRight sourceRight)
            :base(systemCommandHandler)
        {
            UserId = userId;
            SourceId = sourceId;
            SourceKey = sourceKey;
            EventStreamId = eventStreamId;
            SourceRight = sourceRight;
        }
    }
}
