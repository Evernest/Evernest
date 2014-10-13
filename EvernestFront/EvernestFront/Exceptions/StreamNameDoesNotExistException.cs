using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameDoesNotExistException : Exception
    {
        private string StreamName { get; set; }

        public StreamNameDoesNotExistException(string name)
            : base()
        {
            StreamName = name;
        }

    }
}
