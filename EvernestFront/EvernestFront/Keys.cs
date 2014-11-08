using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    static class Keys
    {
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        private const int KeyLength = 16;

        internal static String NewKey()
        {
            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(AllowedChars).ToArray();

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < KeyLength)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < KeyLength; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }
    }
}
