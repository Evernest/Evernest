using System;

namespace EvernestBack
{
    public class Util
    {
        /// <summary>
        /// Reverse the bytes' order.
        /// </summary>
        /// <param name="array">The byte array where bytes must be reversed.</param>
        /// <param name="offset">The offset where the byte must be reversed.</param>
        /// <param name="count">The number of bytes which must be reversed.</param>
        public static void Reverse(Byte[] array, int offset, int count)
        {
            for (UInt16 i = 0; i < count/2; i++)
            {
                var tmp = array[offset + i];
                array[offset + i] = array[offset + count - 1 - i];
                array[offset + count - 1 - i] = tmp;
            }
        }

        /// <summary>
        /// Convert a byte array to an int (assuming the byte array represents an integer in little endian).
        /// </summary>
        /// <param name="src">The byte array.</param>
        /// <param name="offset">The offset where the int should be read.</param>
        /// <returns>The requested int.</returns>
        public static int ToInt(byte[] src, int offset)
        {
            if(!BitConverter.IsLittleEndian)
                Reverse(src, offset, sizeof(int));
            int tmp = BitConverter.ToInt32(src, offset);
            if(!BitConverter.IsLittleEndian)
                Reverse(src, offset, sizeof(int));
            return tmp;
        }

        /// <summary>
        /// Convert a byte array to a long (assuming the byte array represents an integer in little endian).
        /// </summary>
        /// <param name="src">The byte array.</param>
        /// <param name="offset">The offset where the long should be read.</param>
        /// <returns>The requested long.</returns>
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