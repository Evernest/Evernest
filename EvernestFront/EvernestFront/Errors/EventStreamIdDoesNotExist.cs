

using System;

namespace EvernestFront.Errors
{
    public class EventStreamIdDoesNotExist : FrontError
    { 
        public Int64 StreamId { get; private set; }

        internal EventStreamIdDoesNotExist(Int64 id)
        {
            StreamId = id;  
        }

    }
}
