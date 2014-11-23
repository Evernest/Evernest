using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class IdentifyUser : Answer
    {
        public Int64 UserId { get; private set; }

        internal IdentifyUser(Int64 id)
            : base()
        {
            UserId = id;
        }

        internal IdentifyUser(FrontError err)
            : base(err)
        {
            
        }
    }
    
}
