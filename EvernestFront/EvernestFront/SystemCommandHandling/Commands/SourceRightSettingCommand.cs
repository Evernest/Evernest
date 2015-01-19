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

        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!systemCommandHandlerState.UserIdToData.TryGetValue(UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!userRecord.SourceIdToName.ContainsKey(SourceId))
            {
                error = FrontError.SourceIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceRightSetSystemEvent(SourceKey, EventStreamId, SourceRight);
            error = null;
            return true;
        }
    }
}
