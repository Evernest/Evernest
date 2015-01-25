using System;
using System.Text;

//The deserialization logic has been rewritten in EventRange.cs! Refactorization needed?
namespace EvernestBack
{
    /// <summary>An Event, basically a pair (id, message).</summary>
    public class LowLevelEvent
    {
        /// <summary>
        /// Construct the LowLevelEvent.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="requestId"></param>
        public LowLevelEvent(string message, long requestId)
        {
            RequestId = requestId;
            Message = message;
        }
        
        /// <summary>
        /// Return a bitwise representation of the event (in little-endian)
        /// </summary>
        /// <param name="buffSize">The number of bytes to be retrieved.</param>
        /// <returns>The byte array.</returns>
        public byte[] Serialize(out int buffSize) //odd function
        {
            buffSize = Encoding.Unicode.GetByteCount(Message);

            var finalBytes =
                new Byte[buffSize + sizeof(long) + sizeof(int)];

            var reqIdBytes = BitConverter.GetBytes(RequestId);
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

        /// <summary>
        /// The event's id.
        /// </summary>
        public long RequestId { get; protected set; }
        /// <summary>
        /// The event's content.
        /// </summary>
        public string Message { get; protected set; }
    }
}
