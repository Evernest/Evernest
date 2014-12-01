using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class InvalidEventId : FrontError
    {
        public Int64 StreamId { get; private set; }

        public int EventId { get; private set; }

        internal InvalidEventId(int evId, EventStream strm)
        {
            StreamId = strm.Id;
            EventId = evId;
        }
    }
}
