using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    abstract class Agent
    {
        protected UInt64 RequestID { get; private set; }
        protected EventStream feedback;
        public String Message { get; protected set; }

        protected Agent(String message, UInt64 requestID, EventStream feedback)
        {
            this.Message = message;
            this.RequestID = requestID;
            this.feedback = feedback;
        }

        private void Reverse(Byte[] array, int offset, int count)
        {
            Byte tmp;
            for (UInt16 i = 0; i < count; i++)
            {
                tmp = array[offset + i];
                array[offset + i] = array[offset + count - 1 - i];
                array[offset + count - 1 - i] = tmp;
            }
        }

        public Byte[] Serialize(out UInt16 buffSize)
        {
            buffSize = (UInt16) System.Text.Encoding.Unicode.GetByteCount(Message);
            Byte[] finalBytes = new Byte[buffSize+sizeof(UInt64)+sizeof(UInt16)];
            Byte[] reqIDBytes = BitConverter.GetBytes(RequestID);
            Byte[] msgLengthBytes = BitConverter.GetBytes(buffSize);
            System.Text.Encoding.Unicode.GetBytes(Message, 0, Message.Length, finalBytes, (Int32) sizeof(UInt64)+sizeof(UInt16));
            if(!BitConverter.IsLittleEndian) // ensure we use little-endianness
            {
                Reverse(reqIDBytes, 0, sizeof(UInt64));
                Reverse(msgLengthBytes, 0, sizeof(UInt16));
            }
            System.Buffer.BlockCopy(reqIDBytes, 0, finalBytes, 0, sizeof(UInt64));
            System.Buffer.BlockCopy(msgLengthBytes, 0, finalBytes, sizeof(UInt64), sizeof(UInt16));
            buffSize += sizeof(UInt64)+sizeof(UInt16);
            return finalBytes;
        }

        public void ReadFromStream(System.IO.Stream input) //should check whether an error happen when reading
        {
            Byte[] buffer = new Byte[sizeof(UInt64)];
            input.Read(buffer, 0, sizeof(UInt64));
            if (!BitConverter.IsLittleEndian)
                Reverse(buffer, 0, sizeof(UInt64));
            RequestID = BitConverter.ToUInt64(buffer, 0);
            input.Read(buffer, 0, sizeof(UInt16));
            if (!BitConverter.IsLittleEndian)
                Reverse(buffer, 0, sizeof(UInt16));
            UInt16 msgLength = BitConverter.ToUInt16(buffer, 0);
            Byte[] msgBuffer = new Byte[msgLength];
            input.Read(msgBuffer, 0, msgLength);
            Message = System.Text.Encoding.Unicode.GetString(msgBuffer);
        }

        abstract public void Processed();

        public void ProcessFailed(String feedbackMessage)
        {
        }
    }
}
