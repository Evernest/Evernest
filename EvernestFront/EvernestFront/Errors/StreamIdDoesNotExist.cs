

using System;

namespace EvernestFront.Errors
{
    public class StreamIdDoesNotExist : FrontError
    { 
        public Int64 StreamId { get; private set; }

        internal StreamIdDoesNotExist(Int64 id)
        {
            StreamId = id;  
        }

    }
}
