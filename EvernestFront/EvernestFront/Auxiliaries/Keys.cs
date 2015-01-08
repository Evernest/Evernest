using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvernestFront.Auxiliaries
{
    static class Keys
    {
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        private const int KeyLength = 32;
        private const int SaltLength = 32;
        private const int PasswordLength = 32;

        internal static string NewPassword()
        {
            return RandomString(PasswordLength);
        }

        internal static string NewSalt()
        {
            return RandomString(SaltLength);
        }


        internal static string NewKey()
        {
            return RandomString(KeyLength);
        }

        private static string RandomString(int length)
        {
            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(AllowedChars).ToArray();

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
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
