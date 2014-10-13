using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameTakenException :Exception
    {
        private string StreamName { get; set; }

        public StreamNameTakenException(string name)
        :base() 
        {
            StreamName = name;
        }

    }
}
