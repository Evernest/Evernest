using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EvernestFront.Utilities
{
    class PasswordManager
    {
        internal KeyValuePair<string, byte[]> SaltAndHash(string password)
        {
            var keyGenerator = new KeyGenerator();
            var passwordSalt = Encoding.ASCII.GetBytes(keyGenerator.NewSalt());
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var hmacSHA256 = new HMACSHA256(passwordSalt);
            var saltedHash = hmacSHA256.ComputeHash(passwordBytes);
            var saltedPasswordHash = Encoding.ASCII.GetString(saltedHash);
            return new KeyValuePair<string, byte[]>(saltedPasswordHash, passwordSalt);
        }

        internal bool Verify(string password, string hash, byte[] salt)
        {
            var hmacSHA256 = new HMACSHA256(salt);
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var saltedHash = Encoding.ASCII.GetString(hmacSHA256.ComputeHash(passwordBytes));
            return (hash.Equals(saltedHash));
        }

        internal bool StringIsASCII(string password)
        {
            return password.Equals(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(password)));
        }
    }
}
