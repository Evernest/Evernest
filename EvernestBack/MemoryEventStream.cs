using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EvernestBack
{
    
    /// <summary>This is a small test Stream storing messages in a List instead of in Azure.</summary>
    public class MemoryEventStream : IEventStream
    {
        public long Index;
        private readonly List<string> _messages = new List<string>();
        private readonly string _streamFileName;

        public MemoryEventStream(string streamStringID)
        {
            _streamFileName = StreamFileName(streamStringID);
            if (File.Exists(_streamFileName))
            {
                var file = new StreamReader(_streamFileName);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    Index++;
                    _messages.Add(line);
                }
                file.Close();
            }
        }

        public void Push(string message, Action<IAgent> callback, Action<IAgent, String> callbackFailure)
        {
            IAgent a = new MyAgent(message, Index);
            Index++;
            _messages.Add(a.Message);
            callback(a);
        }

        public void Pull(long id, Action<IAgent> callback, Action<IAgent, String> callbackFailure)
        {
            IAgent a = new MyAgent(_messages.ElementAt((int) id), id);
            callback(a);
        }

        public long Size()
        {
            return _messages.Count();
        }

        ~MemoryEventStream()
        {
            var file = new StreamWriter(_streamFileName);
            foreach (var message in _messages)
                file.WriteLine(message);
            file.Close();
        }

        private class MyAgent : IAgent
        {
            public MyAgent(string message, long index)
            {
                Message = message;
                RequestID = index;
            }

            public string Message { get; protected set; }
            public long RequestID { get; private set; }
        }

        private static string StreamFileName(string streamID)
        {
            return streamID + "_RAMStreamContent.txt";
        }

        public static bool StreamExists(string streamID)
        {
            return (File.Exists(StreamFileName(streamID)));
        }

        public static void CreateStream(string streamID)
        {
            var file = new StreamWriter(StreamFileName(streamID));
            file.Close();
        }
    }
}