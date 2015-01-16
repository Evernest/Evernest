using System;


namespace EvernestBack
{
    class Reader:Agent
    {
        System.IO.Stream readStream;
        public Reader(string Message, long RequestID, System.IO.Stream readStream, Action<IAgent> CallbackSuccess, Action<IAgent, String> CallbackFailure)
            :base(Message, RequestID, CallbackSuccess, CallbackFailure) // TODO : remove ulong cast when Agent updated
        {
            this.readStream = readStream;
        }
        
        private void Read()
        {
            ReadFromStream(readStream);
            Processed();
        }
    }
}
