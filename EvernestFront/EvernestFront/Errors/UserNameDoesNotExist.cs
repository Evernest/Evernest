using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class UserNameDoesNotExist : FrontError
    {
        public string Name { get; private set; }

        internal UserNameDoesNotExist(string name)
        {
            Name = name;
        }
    
    }
}
