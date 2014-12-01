using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    class GetEventStream : Answer
    {
        public EventStream EventStream;

        internal GetEventStream(EventStream eventStream)
            : base()
        {
            EventStream = eventStream;
        }

        internal GetEventStream(FrontError err)
            : base(err)
        {
        }
       

    }
}
