using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    interface IRequest
    {
        string ToString();

        IAnswer Process();
    }
}
