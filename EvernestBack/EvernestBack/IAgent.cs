using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestBack
{
    public interface IAgent
    {
        String Message { get; }
        UInt64 RequestID { get; }
    }
}
