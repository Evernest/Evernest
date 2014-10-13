using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class Push : Answer
    {
        public string MessageID { get; private set; }
        
        public Push(Exception exn)
            : base(exn) { }

        public Push(string id)
        {
            success = true;
            MessageID = id;
        }

    }
}
