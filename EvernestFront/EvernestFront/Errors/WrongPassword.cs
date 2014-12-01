using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class WrongPassword : FrontError
    {
        public string UserName { get; private set; }

        public string BadPassword { get; private set; }

        internal WrongPassword(string user, string password)
        {
            UserName = user;
            BadPassword = password;
        }
    }
}
