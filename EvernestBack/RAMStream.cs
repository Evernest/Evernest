using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        public long Index = 0;

        public RAMStream(string streamStringID)
        {
            StreamFileName = streamStringID + "_RAMStreamContent.txt";
            String line;
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

        ~RAMStream()
        {
            StreamWriter file = new StreamWriter(StreamFileName);
            foreach( String message in Messages )
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

        private class MyAgent:IAgent
        {
            public String Message { get; protected set; }
            public long RequestID { get; private set; }

            public MyAgent(string Message, long Index)
            {
                this.Message = Message;
                this.RequestID = Index;
            }
        }
    }
}
