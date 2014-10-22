using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class PushEventResponse
    {
        public string new_token { get; set; }
        public int id { get; set; }
        public int token_timeout { get; set; }

        public void ToPrint()
        {
            Console.WriteLine("New_Token : " + new_token + "Id : " + id + "; Token_Timeout : " + token_timeout);
        }
    }
}
