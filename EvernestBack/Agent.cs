using System;
using System.IO;

namespace EvernestBack
{
    class Agent:IAgent
    {
        private Action<IAgent> Callback;
        public long RequestID { get; private set; }
        public string Message { get; protected set; }

        internal Agent(string message, long requestID, 
            Action<IAgent> callback)
        {            
            Message = message;
            RequestID = requestID;
            Callback = callback;
        }

        public static void Reverse(Byte[] array, int offset, int count)
        {
            Byte tmp;
            for (ushort i = 0; i < count/2; i++)
            {
                tmp = array[offset + i];
                array[offset + i] = array[offset + count - 1 - i];
                array[offset + count - 1 - i] = tmp;
            }
        }

        public Byte[] Serialize(out ushort bufferSize)
        {
            bufferSize = 
                (ushort) System.Text.Encoding.Unicode.GetByteCount(Message);

            Byte[] finalBytes = 
                new Byte[bufferSize+sizeof(long)+sizeof(ushort)];

            Byte[] reqIDBytes = BitConverter.GetBytes(RequestID);
            Byte[] msgLengthBytes = BitConverter.GetBytes(bufferSize);
            System.Text.Encoding.Unicode.GetBytes(Message, 0, Message.Length, 
                finalBytes, sizeof(long)+sizeof(ushort));
            // ensure we use little-endianness
            if(!BitConverter.IsLittleEndian) 
            {
                Reverse(reqIDBytes, 0, sizeof(long));
                Reverse(msgLengthBytes, 0, sizeof(ushort));
            }
            Buffer.BlockCopy(reqIDBytes, 0, finalBytes, 0, 
                sizeof(long));
            Buffer.BlockCopy(msgLengthBytes, 0, finalBytes, 
                sizeof(long), sizeof(ushort));
            bufferSize += sizeof(long)+sizeof(ushort);
            return finalBytes;
        }

        public void ReadFromStream(Stream input) 
            //should check whether an error happen when reading
        {
            Byte[] buffer = new Byte[sizeof(long)];
            input.Read(buffer, 0, sizeof(long));
            if (!BitConverter.IsLittleEndian)
                Reverse(buffer, 0, sizeof(long));
            RequestID = BitConverter.ToInt64(buffer, 0);
            input.Read(buffer, 0, sizeof(ushort));
            if (!BitConverter.IsLittleEndian)
                Reverse(buffer, 0, sizeof(ushort));
            ushort msgLength = BitConverter.ToUInt16(buffer, 0);
            Byte[] msgBuffer = new Byte[msgLength];
            input.Read(msgBuffer, 0, msgLength);
            Message = System.Text.Encoding.Unicode.GetString(msgBuffer);
        }

        internal void Processed()
        {
            Callback(this);
        }

        internal void ProcessFailed(string feedbackMessage)
        {
            //TODO
        }
    }
}
