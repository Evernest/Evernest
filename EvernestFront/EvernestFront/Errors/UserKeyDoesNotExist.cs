using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class UserKeyDoesNotExist : FrontError
    {
        public string Key { get; private set; }

        internal UserKeyDoesNotExist(string key)
        {
            Key = key;
        }
    }
}
