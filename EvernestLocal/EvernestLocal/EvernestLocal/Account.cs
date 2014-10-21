using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class Account
    {
        public string user { get; set; }
        public string password { get; set; }

        public Account(String user, String password)
        {
            this.user = user;
            this.password = password;
        }

        public string ToJsonString()
        {
            return "{\"user\":\"" + user + "\",\"password\":\"" + password + "\"}";
        }

        public void ToPrint()
        {
            Console.WriteLine("User : " + user + "; Password : " + password);
        }
    }
}
