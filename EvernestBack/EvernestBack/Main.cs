using System;


namespace EvernestBack
{
    class Test
    {
        static void Main(string[] args)
        {
            UInt64 MessageIndex = 100;
            IStream Stream = new RAMStream();
            Stream.Push("Banane", a => {
                Console.WriteLine("ACK" + a.RequestID) ; 
                    MessageIndex = a.RequestID;
                }
            );
            Stream.Pull(MessageIndex, a =>
            {
                Console.WriteLine(a.Message);
            });
        }
    }
}