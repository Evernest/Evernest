using System.Collections.Generic;

namespace EvernestFront.SystemCommandHandling
{
    class UserRecord
    {
        internal string UserName;

        internal string SaltedPasswordHash;

        internal byte[] PasswordSalt;

        internal HashSet<string> KeyNames;

        internal HashSet<string> SourceNames;

        internal Dictionary<long, string> SourceIdToName; 

        internal long NextSourceId;

        internal UserRecord(string name, string hash, byte[] salt)
        {
            UserName = name;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
            KeyNames = new HashSet<string>();
            SourceNames = new HashSet<string>();
            SourceIdToName = new Dictionary<long, string>();
            NextSourceId = 0;
        }

    }
}
