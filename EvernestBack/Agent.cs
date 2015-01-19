using System;
using System.IO;
using System.Text;

namespace EvernestBack
{
    /// <summary>
    ///  The role of the Agent class is to implement common processes used by 
    ///  both Reader and Producer. That is for instance : the Callback procedure 
    ///  when processed, the RequestID and Message logic, etc...
    /// </summary>
    internal class Agent : IAgent
    {
        private readonly Action<IAgent, String> _callbackFailure;
        private readonly Action<IAgent> _callbackSuccess;
        internal Agent(string message, long requestId,
            Action<IAgent> callbackSuccess, Action<IAgent, String> callbackFailure)
        {
            Message = message;
            RequestID = requestId;
            _callbackSuccess = callbackSuccess;
            _callbackFailure = callbackFailure;
        }

        public long RequestID { get; private set; }
        public string Message { get; protected set; }

        public Byte[] Serialize(out int buffSize)
        {
            buffSize = Encoding.Unicode.GetByteCount(Message);

            var finalBytes =
                new Byte[buffSize + sizeof (long) + sizeof (int)];

            var reqIdBytes = BitConverter.GetBytes(RequestID);
            var msgLengthBytes = BitConverter.GetBytes(buffSize);
            Encoding.Unicode.GetBytes(Message, 0, Message.Length,
                finalBytes, sizeof (long) + sizeof (int));
            // ensure we use little-endianness
            if (!BitConverter.IsLittleEndian)
            {
                Util.Reverse(reqIdBytes, 0, sizeof (long));
                Util.Reverse(msgLengthBytes, 0, sizeof (int));
            }
            Buffer.BlockCopy(reqIdBytes, 0, finalBytes, 0,
                sizeof (long));
            Buffer.BlockCopy(msgLengthBytes, 0, finalBytes,
                sizeof (long), sizeof (int));
            buffSize += sizeof (long) + sizeof (int);
            return finalBytes;
        }

        public void ReadFromStream(Stream input)
            //should check whether an error happen when reading
        {
            var buffer = new Byte[sizeof (long)];
            input.Read(buffer, 0, sizeof (long));
            if (!BitConverter.IsLittleEndian)
                Util.Reverse(buffer, 0, sizeof (long));
            RequestID = BitConverter.ToInt64(buffer, 0);
            input.Read(buffer, 0, sizeof (int));
            if (!BitConverter.IsLittleEndian)
                Util.Reverse(buffer, 0, sizeof (int));
            var msgLength = BitConverter.ToInt32(buffer, 0);
            var msgBuffer = new Byte[msgLength];
            input.Read(msgBuffer, 0, msgLength);
            Message = Encoding.Unicode.GetString(msgBuffer);
        }

        internal void Processed()
        {
            _callbackSuccess(this);
        }

        internal void ProcessFailed(string feedbackMessage)
        {
            _callbackFailure(this, feedbackMessage);
        }
    }
}
