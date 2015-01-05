using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class InvalidString : FrontError
    {
        public string BadString { get; private set; }

        internal InvalidString(string invalidString)
        {
            BadString = invalidString;
        }
    }
}
