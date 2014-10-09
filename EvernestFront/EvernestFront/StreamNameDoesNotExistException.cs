using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class StreamNameDoesNotExistException :Exception
    {
        private string streamName;

        public StreamNameDoesNotExistException(string name)
            : base()
        {
            streamName = name;
        }

        protected string GetName()
        {
            return streamName;
        }

    }
}
