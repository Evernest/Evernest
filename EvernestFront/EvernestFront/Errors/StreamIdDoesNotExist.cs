

using System;

namespace EvernestFront.Errors
{
    public class StreamIdDoesNotExist : FrontException
    { 
        public Int64 StreamId { get; private set; }
        /// <summary>
        /// Constructor for StreamNameDoesNotExistException.
        /// </summary>
        /// <param name="id"></param>
        internal StreamIdDoesNotExist(Int64 id)
        {
            StreamId = id;  
        }

    }
}
