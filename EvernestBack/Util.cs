using System;

namespace EvernestBack
{
    public class Util
    {
        public static void Reverse(Byte[] array, int offset, int count)
        {
            for (UInt16 i = 0; i < count/2; i++)
            {
                var tmp = array[offset + i];
                array[offset + i] = array[offset + count - 1 - i];
                array[offset + count - 1 - i] = tmp;
            }
        }

        public static int ToInt(byte[] src, int offset)
        {
            if(!BitConverter.IsLittleEndian)
                Reverse(src, offset, sizeof(int));
            int tmp = BitConverter.ToInt32(src, offset);
            if(!BitConverter.IsLittleEndian)
                Reverse(src, offset, sizeof(int));
            return tmp;
        }

        public static long ToLong(byte[] src, int offset)
        {
            if (!BitConverter.IsLittleEndian)
                Reverse(src, offset, sizeof(long));
            long tmp = BitConverter.ToInt64(src, offset);
            if (!BitConverter.IsLittleEndian)
                Reverse(src, offset, sizeof(long));
            return tmp;
        }
    }
}