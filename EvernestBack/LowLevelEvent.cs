using System;
using System.Text;

//The deserialization logic has been rewritten in EventRange.cs! Refactorization needed?
namespace EvernestBack
{
    /// <summary>An Event, basically a pair (id, message).</summary>
    public class LowLevelEvent
    {
        public LowLevelEvent(string message, long requestID)
        {
            RequestID = requestID;
            Message = message;
        }
        
        public byte[] Serialize(out int buffSize)
        {
            buffSize = Encoding.Unicode.GetByteCount(Message);

            var finalBytes =
                new Byte[buffSize + sizeof(long) + sizeof(int)];

            var reqIdBytes = BitConverter.GetBytes(RequestID);
            var msgLengthBytes = BitConverter.GetBytes(buffSize);
            Encoding.Unicode.GetBytes(Message, 0, Message.Length,
                finalBytes, sizeof(long) + sizeof(int));
            // ensure we use little-endianness
            if (!BitConverter.IsLittleEndian)
            {
                Util.Reverse(reqIdBytes, 0, sizeof(long));
                Util.Reverse(msgLengthBytes, 0, sizeof(int));
            }
            Buffer.BlockCopy(reqIdBytes, 0, finalBytes, 0,
                sizeof(long));
            Buffer.BlockCopy(msgLengthBytes, 0, finalBytes,
                sizeof(long), sizeof(int));
            buffSize += sizeof(long) + sizeof(int);
            return finalBytes;
        }

        public long RequestID { get; protected set; }
        public string Message { get; protected set; }
    }
}
