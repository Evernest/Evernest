using System;
using System.IO;

namespace EvernestBack
{
    internal class Reader : Agent
    {
        private readonly Stream _readStream;

        public Reader(string message, long requestId, Stream readStream,
            Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
            : base(message, requestId, callbackSuccess, callbackFailure) // TODO : remove ulong cast when Agent updated
        {
            _readStream = readStream;
        }

        private void Read()
        {
            ReadFromStream(_readStream);
            Processed();
        }
    }
}