using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace EvernestBack
{
    /// <summary>This is a small test Stream storing messages in a List instead of in Azure.</summary>
    public class MemoryEventStream : IEventStream
    {
        private long _index;
        private readonly List<string> _messages = new List<string>();
        private readonly string _streamFileName;

        /// <summary>
        /// Construct the MemoryEventStream.
        /// </summary>
        /// <param name="store">The stream manager.</param>
        /// <param name="streamStringId">The stream's name.</param>
        public MemoryEventStream(AzureStorageClient store, string streamStringId)
        {
            _streamFileName = StreamFileName(store, streamStringId);
            if (File.Exists(_streamFileName))
            {
                var file = new StreamReader(_streamFileName);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    _index++;
                    _messages.Add(line);
                }
                file.Close();
            }
        }

        public void Push(string message, Action<LowLevelEvent> success, Action<string, string> failure)
        {
            LowLevelEvent a = new LowLevelEvent(message, _index);
            _index++;
            _messages.Add(a.Message);
            success(a);
        }

        public void Pull(long id, Action<LowLevelEvent> callback, Action<long, string> callbackFailure)
        {
            if (_index > id)
                callback(new LowLevelEvent(_messages.ElementAt((int) id), id));
            else
                callbackFailure(id, "Can not fetch event.");
        }

        public void PullRange(long firstId, long lastId, Action<IEnumerable<LowLevelEvent>> success, Action<long, long, string> failure)
        {
            if (firstId >= 0 && firstId <= lastId && _index > lastId)
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

        /// <summary>
        /// Create a file name with the stream's name.
        /// </summary>
        /// <param name="store">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        /// <returns></returns>
        private static string StreamFileName(AzureStorageClient store, string streamId)
        {
            string r = store.DummyDataPath + streamId + "_RAMStreamContent.txt";
            //string r = "c:\\EvernestData\\" + streamId + "_RAMStreamContent.txt";
            Console.WriteLine("Stream file name: " + r);
            return r;
        }

        /// <summary>
        /// Tell whether the stream exists or not.
        /// </summary>
        /// <param name="store">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        /// <returns></returns>
        public static bool StreamExists(AzureStorageClient store, string streamId)
        {
            return File.Exists(StreamFileName(store, streamId));
        }

        /// <summary>
        /// Create a stream.
        /// </summary>
        /// <param name="store">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        public static void CreateStream(AzureStorageClient store, string streamId)
        {
            string fn = StreamFileName(store, streamId);
            var file = new StreamWriter(fn);
            file.Close();
        }

        /// <summary>
        /// Delete a stream if it already exists.
        /// </summary>
        /// <param name="store">The stream manager.</param>
        /// <param name="streamId">The stream's name.</param>
        internal static void DeleteStream(AzureStorageClient store, string streamId)
        {
            File.Delete(StreamFileName(store, streamId));
        }

        /// <summary>
        /// Clear all the existing MemoryEventStream.
        /// </summary>
        /// <param name="store"></param>
        public static void ClearAll(AzureStorageClient store)
        {
            var list = new DirectoryInfo(store.DummyDataPath);
            foreach (FileInfo file in list.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Request a server update
        /// </summary>
        public void Update()
        {
            //TODO: you may want to start writing the file here
        }
    }
}