using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class Request
    {
        public string content { get; set; }
        public string token { get; set; }

        public Request(String content, String token)
        {
            this.content = content;
            this.token = token;
        }

        public void ToPrint()
        {
            Console.WriteLine("Token : " + token + "; Content : " + content);
        }
    }
}
