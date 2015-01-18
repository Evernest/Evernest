using System;

namespace EvernestBack
{
    public class Util
    {
        public static void Reverse(Byte[] array, int offset, int count)
        {
            for (UInt16 i = 0; i < count; i++)
            {
                var tmp = array[offset + i];
                array[offset + i] = array[offset + count - 1 - i];
                array[offset + count - 1 - i] = tmp;
            }
        }
    }
}