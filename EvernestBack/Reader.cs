using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace EvernestBack
{
    class Reader:Agent
    {
        System.IO.Stream readStream;
        public Reader(String Message, UInt64 RequestID, System.IO.Stream readStream, Action<IAgent> Callback)
            :base(Message, RequestID, Callback)
        {
            this.readStream = readStream;
        }
        
        private void Read()
        {
            ReadFromStream(readStream);
            Processed();
        }
    }
}
