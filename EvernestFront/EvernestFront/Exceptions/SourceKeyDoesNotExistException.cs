using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    public class SourceKeyDoesNotExistException : FrontException
    {
        public String Key { get; private set; }

        internal SourceKeyDoesNotExistException(String key)
        {
            Key = key;
        }
    }
}
