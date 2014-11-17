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

        public Byte[] Serialize(out UInt16 buffSize)
        {
            Byte[] reqIDBytes = BitConverter.GetBytes(RequestID);
            Byte[] msgBytes = System.Text.Encoding.Unicode.GetBytes(Message);
            buffSize = (UInt16) System.Text.Encoding.Unicode.GetByteCount(Message);
            Byte[] msgLengthBytes = BitConverter.GetBytes(buffSize);
            if(!BitConverter.IsLittleEndian) // ensure we use little-endianness
            {
                Byte tmp;
                for (UInt16 i = 0; i < 4; i++)
                {
                    tmp = reqIDBytes[i];
                    reqIDBytes[i] = reqIDBytes[7-i];
                    reqIDBytes[7 - i] = reqIDBytes[i];
                }
                tmp = msgLengthBytes[0];
                msgLengthBytes[0] = msgLengthBytes[1];
                msgLengthBytes[1] = tmp;
            }
            Byte[] finalBytes = new Byte[buffSize];
            System.Buffer.BlockCopy(reqIDBytes, 0, finalBytes, 0, 8);
            System.Buffer.BlockCopy(msgLengthBytes, 0, finalBytes, 8, 2);
            System.Buffer.BlockCopy(msgBytes, 0, finalBytes, 10, buffSize);
            buffSize += 10;
            return finalBytes;
        }

        abstract public void Processed();

        public void ProcessFailed(String feedbackMessage)
        {
        }
    }
}
