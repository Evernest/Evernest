using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class Push : Answer
    {
        public Push(Exception exn)
            : base(exn)
        {
        }
        public Push()
        {
            success = true;
        }

    }
}
