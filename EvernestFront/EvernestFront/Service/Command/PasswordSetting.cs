using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Service.Command
{
    class PasswordSetting : CommandBase
    {
        internal long UserId { get; private set; }
        
        internal string CurrentPassword { get; private set; }

        internal string NewPassword { get; private set; }

    }
}
