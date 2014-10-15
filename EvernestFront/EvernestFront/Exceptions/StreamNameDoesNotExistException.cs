using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameDoesNotExistException : Exception
    { 
        public string StreamName { get; private set; }
        /// <summary>
        /// Constructor for StreamNameDoesNotExistException.
        /// </summary>
        /// <param name="name"></param>
        public StreamNameDoesNotExistException(string name)
        {
            StreamName = name;
        }

    }
}
