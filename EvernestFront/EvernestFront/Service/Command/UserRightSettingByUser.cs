using System.Collections.Generic;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Service.Command
{
    class UserRightSettingByUser : CommandBase
    {
        internal string TargetName { get; private set; }

        internal long EventStreamId { get; private set; }

        internal string AdminName { get; private set; }

        internal AccessRight Right { get; private set; }

        internal UserRightSettingByUser(CommandReceiver commandReceiver, string targetName, long eventStreamId,
            string adminName, AccessRight right)
            : base(commandReceiver)
        {
            TargetName = targetName;
            EventStreamId = eventStreamId;
            AdminName = adminName;
            Right = right;
        }

        public override bool TryToSystemEvent(ServiceData serviceData, out Contract.SystemEvent.ISystemEvent systemEvent, out FrontError? error)
        {
            HashSet<string> eventStreamAdmins;
            if (!serviceData.EventStreamIdToAdmins.TryGetValue(EventStreamId, out eventStreamAdmins))
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
            systemEvent= new UserRightSet(EventStreamId, TargetName, Right);
            error = null;
            return true;
        }
    }
}

