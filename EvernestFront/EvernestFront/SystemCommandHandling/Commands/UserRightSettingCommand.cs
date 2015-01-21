using System.Collections.Generic;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class UserRightSettingCommand : CommandBase
    {
        internal string TargetName { get; private set; }

        internal long EventStreamId { get; private set; }

        internal string AdminName { get; private set; }

        internal long AdminId { get; private set; }

        internal AccessRight Right { get; private set; }

        internal UserRightSettingCommand(SystemCommandHandler systemCommandHandler, string targetName,
            long eventStreamId, string adminName, long adminId, AccessRight right)
            : base(systemCommandHandler)
        {
            AdminId = adminId;
            TargetName = targetName;
            EventStreamId = eventStreamId;
            AdminName = adminName;
            Right = right;
        }
    }
}

