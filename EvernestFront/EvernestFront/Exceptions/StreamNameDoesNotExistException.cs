using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameDoesNotExistException : Exception
    {
        public string StreamName { get; private set; }

        public StreamNameDoesNotExistException(string name)
        {
            StreamName = name;
        }

    }
}
