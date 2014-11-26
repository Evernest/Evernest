using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    class GetUser : Answer
    {
        public User User;

        internal GetUser(User user)
            : base()
        {
            User = user;
        }

        internal GetUser(FrontError err)
            : base(err)
        {
        }

    }
}
