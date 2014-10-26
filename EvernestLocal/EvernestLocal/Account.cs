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

        public void ToPrint()
        {
            Console.WriteLine("User : " + user + "; Password : " + password);
        }
    }
}
