using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    class UserKeyNameTaken : FrontError
    {
        //user UserId already owns a key called KeyName, he cannot create another one.

        public Int64 UserId { get; private set; }
        public string KeyName { get; private set; }

        internal UserKeyNameTaken(Int64 userId, string keyName)
        {
            UserId = userId;
            KeyName = keyName;
        }
    }
}
