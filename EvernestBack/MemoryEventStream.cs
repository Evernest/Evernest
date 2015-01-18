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

        public MemoryEventStream(AzureStorageClient store, string streamStringID)
        {
            _streamFileName = StreamFileName(store, streamStringID);
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

        private static string StreamFileName(AzureStorageClient store, string streamID)
        {
            return store.DummyDataPath + streamID + "_RAMStreamContent.txt";
        }

        public static bool StreamExists(AzureStorageClient store, string streamID)
        {
            return File.Exists(StreamFileName(store, streamID));
        }

        public static void CreateStream(AzureStorageClient store, string streamID)
        {
            string fn = StreamFileName(store, streamID);
            var file = new StreamWriter(fn);
            file.Close();
        }
    }
}