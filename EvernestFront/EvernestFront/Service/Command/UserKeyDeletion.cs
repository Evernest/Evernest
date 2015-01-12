using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Service.Command
{
    class UserKeyDeletion : CommandBase
    {
        internal string Key { get; set; } //base64 encoded int
        
        internal long UserId { get; set; }
        
        internal string KeyName { get; set; }
    }
}
