using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class RequestToken
    {
        public string token { get; set; }

        public RequestToken(String token)
        {
            this.token = token;
        }

        public void ToPrint()
        {
            Console.WriteLine("Token : " + token);
        }
    }
}
