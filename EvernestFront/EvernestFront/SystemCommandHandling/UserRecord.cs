using System.Collections.Generic;

namespace EvernestFront.SystemCommandHandling
{
    /// <summary>
    /// Data stored in SystemCommandHandlerState for each user.
    /// </summary>
    class UserRecord
    {
        internal string UserName;

        internal string SaltedPasswordHash;

        internal byte[] PasswordSalt;

        internal HashSet<long> RelatedEventStreams;

        internal HashSet<string> KeyNames;

        internal HashSet<string> SourceNames;

        internal Dictionary<long, string> SourceIdToName; 

        internal long NextSourceId;

        internal UserRecord(string name, string hash, byte[] salt)
        {
            UserName = name;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
            RelatedEventStreams = new HashSet<long>();
            KeyNames = new HashSet<string>();
            SourceNames = new HashSet<string>();
            SourceIdToName = new Dictionary<long, string>();
            NextSourceId = 0;
        }

    }
}
