using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class StreamNameTakenException :Exception
    {
        private string streamName;

        public StreamNameTakenException(string name)
        :base() 
        {
            streamName = name;
        }

        protected string GetName()
        {
            return streamName;
        }
    }
}
