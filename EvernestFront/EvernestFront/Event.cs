using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    public class Event
    {
        public Int64 Id { get; private set; }

        public String Message { get; private set; }

        public String ParentStream { get; private set; }

        internal Event(Int64 eventId, String message, StreamInfo parentStream)
        {
            Id = eventId;
            Message = message;
            ParentStream = parentStream.Name;
        }
    }
}
