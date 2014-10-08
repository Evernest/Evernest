using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    namespace Request
    {
        class PullBetween : Request
        {
            string eventIdFrom;
            string eventIdTo;

            public PullBetween(string user, string streamName, string from, string to)
                : base(user, streamName)
            {
                this.eventIdFrom = from;
                this.eventIdTo = to;
            }

            public override IAnswer Process()
            {
                throw new NotImplementedException();
            }
        } 
    }
}
