using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameTakenException :Exception
    {
        public string StreamName { get; private set; }

        public StreamNameTakenException(string name)
        {
            StreamName = name;
        }

    }
}
