using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Service.Command
{
    class UserDeletion : CommandBase
    {
        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal string Password { get; private set; }
    }
}
