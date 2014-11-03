using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Answers
{
    public class AddUser : Answer
    {
        internal AddUser(FrontException exn)
            : base(exn) { }

        internal AddUser()
            : base() { }
    }
}
