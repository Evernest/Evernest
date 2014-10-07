using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class RequestPush:Request
    {
        private Event eventToPush;

        public RequestPush(string user, string streamName,Event eventToPush)
            : base(user, streamName)
        {
            this.eventToPush = eventToPush;
        }
    }
}
