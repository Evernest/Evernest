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
        private String streamFileName;
        List<string> Messages = new List<string>();
        public UInt64 Index = 0;

        public RAMStream(String streamStringID)
        {
            streamFileName = streamStringID + "_RAMStreamContent.txt";
            String line;
            if (System.IO.File.Exists(streamFileName))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(streamFileName);
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
            System.IO.StreamWriter file = new System.IO.StreamWriter(streamFileName);
            foreach( String message in Messages )
                file.WriteLine(message);
            file.Close();
        }

        public void Push(String Message, Action<IAgent> Callback)
        {
            IAgent a = new MyAgent(Message, Index);
            Index++;
            Messages.Add(a.Message);
            Callback(a);
        }

        public void Pull(UInt64 Id, Action<IAgent> Callback)
        {
            IAgent a = new MyAgent(Messages.ElementAt((int) Id), Id);
            Callback(a);
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
