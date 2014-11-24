using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class SetPassword : Answer
    {
        public Int64 User { get; private set; }
        public String NewPassword { get; private set; }

        internal SetPassword(Int64 user, string password)
            : base()
        {
            User = user;
            NewPassword = password;
        }

        internal SetPassword(FrontError err)
            : base(err)
        {
            
        }

    }
}
