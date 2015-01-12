using System;
using System.Collections.Generic;
using System.Linq;

namespace EvernestBack
{

    /**
     * This is a small test Stream storing messages in a List instead of in
     * Azure.
     */
    public class RAMStream:IEventStream
    {
        private string StreamFileName;
        List<string> Messages = new List<string>();
        public long Index = 0;

        public RAMStream(string streamStringID)
        {
            StreamFileName = streamStringID + "_RAMStreamContent.txt";
            string line;
            if (System.IO.File.Exists(StreamFileName))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(StreamFileName);
                while ((line = file.ReadLine()) != null)
                {
                    Index++;
                    Messages.Add(line);
                }
                file.Close();
            }
        }

        ~RAMStream()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(StreamFileName);
            foreach( string message in Messages )
                file.WriteLine(message);
            file.Close();
        }

        public void Push(string message, Action<IAgent> callback, Action<IAgent, String> callbackfailure)
        {
            IAgent a = new MyAgent(message, Index);
            Index++;
            Messages.Add(a.Message);
            callback(a);
        }

        public void Pull(long id, Action<IAgent> callback, Action<IAgent, String> callbackfailure)
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

            public MyAgent(string Message, long Index)
            {
                this.Message = Message;
                this.RequestID = Index;
            }
        }
    }
}
