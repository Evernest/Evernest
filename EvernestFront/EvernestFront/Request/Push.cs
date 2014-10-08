using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    namespace Request
    {
        class Push : Request
        {
            private Event eventToPush;

            public Push(string user, string streamName, Event eventToPush)
                : base(user, streamName)
            {
                this.eventToPush = eventToPush;
            }

            public override IAnswer Process()
            {
                throw new NotImplementedException();
            }
        } 
    }
}
