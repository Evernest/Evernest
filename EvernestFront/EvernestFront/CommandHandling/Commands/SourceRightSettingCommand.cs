using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling.Commands
{
    class SourceRightSettingCommand : CommandBase
    {
        internal readonly long SourceId;

        internal readonly long UserId;

        internal readonly string SourceKey;

        internal readonly long EventStreamId;

        internal readonly AccessRight SourceRight;

        internal SourceRightSettingCommand(CommandHandler commandHandler, long userId, long sourceId, 
            string sourceKey, long eventStreamId, AccessRight sourceRight)
            :base(commandHandler)
        {
            UserId = userId;
            SourceId = sourceId;
            SourceKey = sourceKey;
            EventStreamId = eventStreamId;
            SourceRight = sourceRight;
        }

        public override bool TryToSystemEvent(CommandHandlingData serviceData, out ISystemEvent systemEvent, out FrontError? error)
        {
            CommandHandlingUserData userData;
            if (!serviceData.UserIdToData.TryGetValue(UserId, out userData))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!userData.SourceIdToName.ContainsKey(SourceId))
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
