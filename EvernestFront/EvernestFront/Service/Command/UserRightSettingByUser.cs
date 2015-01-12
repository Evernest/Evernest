using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Service.Command
{
    class UserRightSettingByUser : CommandBase
    {
        internal string TargetName { get; private set; }

        internal long EventStreamId { get; private set; }

        internal string AdminName { get; private set; }

        internal AccessRights Right { get; private set; }
    }
}
