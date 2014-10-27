using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameDoesNotExistException : FrontException
    { 
        public string StreamName { get; private set; }
        /// <summary>
        /// Constructor for StreamNameDoesNotExistException.
        /// </summary>
        /// <param name="name"></param>
        internal StreamNameDoesNotExistException(string name)
        {
            StreamName = name;
        }

    }
}
