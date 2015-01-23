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

        public void Push(string message, Action<LowLevelEvent> success, Action<string, string> failure)
        {
            LowLevelEvent a = new LowLevelEvent(message, Index);
            Index++;
            _messages.Add(a.Message);
            success(a);
        }

        public void Pull(long id, Action<LowLevelEvent> callback, Action<long, string> callbackFailure)
        {
            if (Index > id)
                callback(new LowLevelEvent(_messages.ElementAt((int) id), id));
            else
                callbackFailure(id, "Can not fetch event.");
        }

        public void PullRange(long firstId, long lastId, Action<IEnumerable<LowLevelEvent>> success, Action<long, long, string> failure)
        {
            if (firstId >= 0 && firstId <= lastId && Index > lastId)
            {
                List<LowLevelEvent> answer = new List<LowLevelEvent>();
                for (var id = firstId; id <= lastId; id++)
                {
                    answer.Add(new LowLevelEvent(_messages.ElementAt((int)id), id));
                }
                success(answer);
            }
            else
            {
                failure(firstId, lastId, "Invalid range.");
            }
        }

        public void FlushPullRequests()
        {}

        public void FlushPushRequests()
        {}

        public long Size()
        {
            return _messages.Count();
        }

        public void Dispose()
        {
            var file = new StreamWriter(_streamFileName);
            foreach (var message in _messages)
                file.WriteLine(message);
            file.Close();
        }

        private static string StreamFileName(AzureStorageClient store, string streamID)
        {
            string r = store.DummyDataPath + streamID + "_RAMStreamContent.txt";
            //string r = "c:\\EvernestData\\" + streamID + "_RAMStreamContent.txt";
            Console.WriteLine("Stream file name: " + r);
            return r;
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

        internal static void DeleteStream(AzureStorageClient store, string streamID)
        {
            File.Delete(StreamFileName(store, streamID));
        }

        public static void ClearAll(AzureStorageClient store)
        {
            var list = new DirectoryInfo(store.DummyDataPath);
            foreach (FileInfo file in list.GetFiles())
            {
                file.Delete();
            }
        }
    }
}