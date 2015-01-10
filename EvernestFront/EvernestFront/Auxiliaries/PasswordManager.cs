using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EvernestFront.Auxiliaries
{
    class PasswordManager
    {
        internal KeyValuePair<string, byte[]> SaltAndHash(string password)
        {
            var keyGenerator = new KeyGenerator();
            var passwordSalt = Encoding.ASCII.GetBytes(keyGenerator.NewSalt());
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var hmacMD5 = new HMACMD5(passwordSalt);
            var saltedHash = hmacMD5.ComputeHash(passwordBytes);
            var saltedPasswordHash = Encoding.ASCII.GetString(saltedHash);
            return new KeyValuePair<string, byte[]>(saltedPasswordHash, passwordSalt);
        }

        internal bool Verify(string password, string hash, byte[] salt)
        {
            var hmacMD5 = new HMACMD5(salt);
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var saltedHash = Encoding.ASCII.GetString(hmacMD5.ComputeHash(passwordBytes));
            return (hash.Equals(saltedHash));
        }
    }
}
