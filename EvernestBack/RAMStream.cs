using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestBack
{

    /**
     * This is a small test Stream storing messages in a List instead of in
     * Azure.
     */
    public class RAMStream:IEventStream
    {
        private String StreamFileName;
        List<string> Messages = new List<string>();
        public UInt64 Index = 0;

        public RAMStream(String streamStringID)
        {
            StreamFileName = streamStringID + "_RAMStreamContent.txt";
            String line;
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
            foreach( String message in Messages )
                file.WriteLine(message);
            file.Close();
        }

        public void Push(String message, Action<IAgent> callback)
        {
            IAgent a = new MyAgent(message, Index);
            Index++;
            Messages.Add(a.Message);
            callback(a);
        }

        public void Pull(UInt64 id, Action<IAgent> callback)
        {
            IAgent a = new MyAgent(Messages.ElementAt((int) id), id);
            callback(a);
        }

        private class MyAgent:IAgent
        {
            public String Message { get; protected set; }
            public UInt64 RequestID { get; private set; }

            public MyAgent(string Message, ulong Index)
            {
                this.Message = Message;
                this.RequestID = Index;
            }
        }
    }
}
