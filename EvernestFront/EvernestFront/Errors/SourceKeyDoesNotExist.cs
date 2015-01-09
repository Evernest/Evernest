using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class SourceKeyDoesNotExist : FrontError
    {
        public string Key { get; private set; }

        internal SourceKeyDoesNotExist(string key)
        {
            Key = key;
        }
    }
}
