﻿using System.Collections.Generic;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.CommandHandling.Commands
{
    class UserRightSettingCommand : CommandBase
    {
        internal string TargetName { get; private set; }

        internal long EventStreamId { get; private set; }

        internal string AdminName { get; private set; }

        internal AccessRight Right { get; private set; }

        internal UserRightSettingCommand(CommandHandler commandHandler, string targetName, long eventStreamId,
            string adminName, AccessRight right)
            : base(commandHandler)
        {
            TargetName = targetName;
            EventStreamId = eventStreamId;
            AdminName = adminName;
            Right = right;
        }

        public override bool TryToSystemEvent(CommandHandlingData commandHandlingData, out ISystemEvent systemEvent, out FrontError? error)
        {
            HashSet<string> eventStreamAdmins;
            if (!commandHandlingData.EventStreamIdToAdmins.TryGetValue(EventStreamId, out eventStreamAdmins))
            {
                error=FrontError.EventStreamIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!eventStreamAdmins.Contains(AdminName))
            {
                error=FrontError.AdminAccessDenied;
                systemEvent = null;
                return false;
            }
            if (eventStreamAdmins.Contains(TargetName))
            {
                error=FrontError.CannotDestituteAdmin;
                systemEvent = null;
                return false;
            }
            systemEvent= new UserRightSetSystemEvent(EventStreamId, TargetName, Right);
            error = null;
            return true;
        }
    }
}

