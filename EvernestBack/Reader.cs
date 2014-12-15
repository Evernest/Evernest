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
        public Reader(string Message, long RequestID, System.IO.Stream readStream, Action<IAgent> Callback)
            :base(Message, (ulong) RequestID, Callback) // TODO : remove ulong cast when Agent updated
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
