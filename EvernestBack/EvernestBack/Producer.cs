using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    class Producer:Agent
    {
        private WriteLocker writer;

        internal Producer(String message, UInt64 requestId, 
            WriteLocker writeLocker, Action<IAgent> Callback)
            :base(message, requestId, Callback)
        {
            this.writer = writeLocker;
            write();
        }
        
        private void write()
        {
            writer.Register(this);
        }
    }
}
