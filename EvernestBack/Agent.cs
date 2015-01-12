using System;

namespace EvernestBack
{
    /* The role of the Agent class is to implement common processes used by 
     * both Reader and Producer. That is for instance : the Callback procedure 
     * when processed, the RequestID and Message logic, etc...
     */

    class Agent:IAgent
    {
        private Action<IAgent> CallbackSuccess;
        private Action<IAgent, String> CallbackFailure;
        public long RequestID { get; private set; }
        public string Message { get; protected set; }

        internal Agent(string Message, long RequestID, 
            Action<IAgent> CallbackSuccess, Action<IAgent, String> CallbackFailure)
        {            
            this.Message = Message;
            this.RequestID = RequestID;
            this.CallbackSuccess = CallbackSuccess;
            this.CallbackFailure = CallbackFailure;
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
                new Byte[BuffSize+sizeof(long)+sizeof(UInt16)];

            Byte[] ReqIDBytes = BitConverter.GetBytes(RequestID);
            Byte[] MsgLengthBytes = BitConverter.GetBytes(BuffSize);
            System.Text.Encoding.Unicode.GetBytes(Message, 0, Message.Length, 
                FinalBytes, (int) sizeof(long)+sizeof(UInt16));
            // ensure we use little-endianness
            if(!BitConverter.IsLittleEndian) 
            {
                Reverse(ReqIDBytes, 0, sizeof(long));
                Reverse(MsgLengthBytes, 0, sizeof(UInt16));
            }
            System.Buffer.BlockCopy(ReqIDBytes, 0, FinalBytes, 0, 
                sizeof(long));
            System.Buffer.BlockCopy(MsgLengthBytes, 0, FinalBytes, 
                sizeof(long), sizeof(UInt16));
            BuffSize += sizeof(long)+sizeof(UInt16);
            return FinalBytes;
        }

        public void ReadFromStream(System.IO.Stream Input) 
            //should check whether an error happen when reading
        {
            Byte[] Buffer = new Byte[sizeof(long)];
            Input.Read(Buffer, 0, sizeof(long));
            if (!BitConverter.IsLittleEndian)
                Reverse(Buffer, 0, sizeof(long));
            RequestID = BitConverter.ToInt64(Buffer, 0);
            Input.Read(Buffer, 0, sizeof(UInt16));
            if (!BitConverter.IsLittleEndian)
                Reverse(Buffer, 0, sizeof(UInt16));
            UInt16 MsgLength = BitConverter.ToUInt16(Buffer, 0);
            Byte[] MsgBuffer = new Byte[MsgLength];
            Input.Read(MsgBuffer, 0, MsgLength);
            Message = System.Text.Encoding.Unicode.GetString(MsgBuffer);
        }

        internal void Processed()
        {
            CallbackSuccess(this);
        }

        internal void ProcessFailed(string FeedbackMessage)
        {
            CallbackFailure(this, FeedbackMessage);
        }
    }
}
