using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestBack
{
    /**
     * The IAgent interface represent a transaction with a Stream. It contains 
     * a message and a unique ID.
     */
    public interface IAgent
    {
        String Message { get; }
        UInt64 RequestID { get; }
    }
}
