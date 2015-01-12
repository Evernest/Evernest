using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Service.Command
{
    class UserCreation : CommandBase
    {
        internal string UserName { get; private set; }

        internal string Password { get; private set; }
    }
}
