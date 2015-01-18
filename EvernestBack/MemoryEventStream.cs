using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace EvernestBack
{

    /**
     * This is a small test Stream storing messages in a List instead of in
     * Azure.
     */
    public class MemoryEventStream:IEventStream
    {
        private string StreamFileName;
        List<string> Messages = new List<string>();
        public long Index;

        public MemoryEventStream(string streamStringId)
        {
            StreamFileName = streamStringId + "_RAMStreamContent.txt";
            string line;
            if (File.Exists(StreamFileName))
            {
                StreamReader file = new StreamReader(StreamFileName);
                while ((line = file.ReadLine()) != null)
                {
                    Index++;
                    Messages.Add(line);
                }
                file.Close();
            }
        }

        ~MemoryEventStream()
        {
            StreamWriter file = new StreamWriter(StreamFileName);
            foreach( string message in Messages )
                file.WriteLine(message);
            file.Close();
        }

        public void Push(string message, Action<IAgent> callback)
        {
            IAgent a = new MyAgent(message, Index);
            Index++;
            Messages.Add(a.Message);
            callback(a);
        }

        public void Pull(long id, Action<IAgent> callback)
        {
            IAgent a = new MyAgent(Messages.ElementAt((int) id), id);
            callback(a);
        }

        public long Size()
        {
            return Messages.Count();
        }

        private class MyAgent:IAgent
        {
            public string Message { get; protected set; }
            public long RequestID { get; private set; }

            public MyAgent(string message, long index)
            {
                Message = message;
                RequestID = index;
            }
        }
    }
}
