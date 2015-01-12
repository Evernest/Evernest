using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Service
{
    class UserDataForService
    {
        internal string UserName { get; set; }
        
        internal string SaltedPasswordHash { get; set; }
        
        internal byte[] PasswordSalt { get; set; }
        
        internal HashSet<string> Keys { get; set; }
        
        internal HashSet<string> Sources { get; set; }

        internal UserDataForService(string name, string hash, byte[] salt, HashSet<string> keys, HashSet<string> sources)
        {
            UserName = name;
            SaltedPasswordHash = hash;
            PasswordSalt = salt;
            Keys = keys;
            Sources = sources;
        }

    }
}
