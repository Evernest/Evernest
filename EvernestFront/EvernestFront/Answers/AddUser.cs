using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class AddUser : Answer
    {
        internal AddUser(FrontError err)
            : base(err) { }

        internal AddUser()
            : base() { }
    }
}
