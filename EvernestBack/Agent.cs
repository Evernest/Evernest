using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestBack
{
    /* The role of the Agent class is to implement common processes used by 
     * both Reader and Producer. That is for instance : the Callback procedure 
     * when processed, the RequestID and Message logic, etc...
     */

    class Agent:IAgent
    {
        private Action<IAgent> Callback;
        public long RequestID { get; private set; }
        public string Message { get; protected set; }

        internal Agent(string Message, long RequestID, 
            Action<IAgent> Callback)
        {            
            this.Message = Message;
            this.RequestID = RequestID;
            this.Callback = Callback;
        }

        public static void Reverse(Byte[] Array, int Offset, int Count)
        {
            Byte Tmp;
            for (UInt16 i = 0; i < Count; i++)
            {
                Tmp = Array[Offset + i];
                Array[Offset + i] = Array[Offset + Count - 1 - i];
                Array[Offset + Count - 1 - i] = Tmp;
            }
        }

        public Byte[] Serialize(out UInt16 BuffSize)
        {
            BuffSize = 
                (UInt16) System.Text.Encoding.Unicode.GetByteCount(Message);

            Byte[] FinalBytes = 
                new Byte[BuffSize+sizeof(UInt64)+sizeof(UInt16)];

            Byte[] ReqIDBytes = BitConverter.GetBytes(RequestID);
            Byte[] MsgLengthBytes = BitConverter.GetBytes(BuffSize);
            System.Text.Encoding.Unicode.GetBytes(Message, 0, Message.Length, 
                FinalBytes, (Int32) sizeof(UInt64)+sizeof(UInt16));
            // ensure we use little-endianness
            if(!BitConverter.IsLittleEndian) 
            {
                Reverse(ReqIDBytes, 0, sizeof(UInt64));
                Reverse(MsgLengthBytes, 0, sizeof(UInt16));
            }
            System.Buffer.BlockCopy(ReqIDBytes, 0, FinalBytes, 0, 
                sizeof(UInt64));
            System.Buffer.BlockCopy(MsgLengthBytes, 0, FinalBytes, 
                sizeof(UInt64), sizeof(UInt16));
            BuffSize += sizeof(UInt64)+sizeof(UInt16);
            return FinalBytes;
        }

        public void ReadFromStream(System.IO.Stream Input) 
            //should check whether an error happen when reading
        {
            Byte[] Buffer = new Byte[sizeof(Int64)];
            Input.Read(Buffer, 0, sizeof(Int64));
            if (!BitConverter.IsLittleEndian)
                Reverse(Buffer, 0, sizeof(Int64));
            RequestID = BitConverter.ToInt64(Buffer, 0);
            Input.Read(Buffer, 0, sizeof(Int16));
            if (!BitConverter.IsLittleEndian)
                Reverse(Buffer, 0, sizeof(Int16));
            UInt16 MsgLength = BitConverter.ToUInt16(Buffer, 0);
            Byte[] MsgBuffer = new Byte[MsgLength];
            Input.Read(MsgBuffer, 0, MsgLength);
            Message = System.Text.Encoding.Unicode.GetString(MsgBuffer);
        }

        internal void Processed()
        {
            Callback(this);
        }

        internal void ProcessFailed(string FeedbackMessage)
        {
            //TODO
        }
    }
}
