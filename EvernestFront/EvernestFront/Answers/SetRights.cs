using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class SetRights:Answer
    {
        public SetRights(Exception exn)
            : base(exn) { }
        public SetRights()
        {
            success = true;
        }
    }
}
