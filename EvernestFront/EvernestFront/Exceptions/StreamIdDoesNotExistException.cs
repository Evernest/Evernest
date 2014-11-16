

using System;

namespace EvernestFront.Exceptions
{
    public class StreamIdDoesNotExistException : FrontException
    { 
        public Int64 StreamId { get; private set; }
        /// <summary>
        /// Constructor for StreamNameDoesNotExistException.
        /// </summary>
        /// <param name="id"></param>
        internal StreamIdDoesNotExistException(Int64 id)
        {
            StreamId = id;  
        }

    }
}
