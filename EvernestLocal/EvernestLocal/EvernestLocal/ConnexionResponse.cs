using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class ConnexionResponse
    {
        public string new_token { get; set; }
        public int token_timeout { get; set; }

        public void ToPrint()
        {
            Console.WriteLine("New_Token : " + new_token + "; Token_Timeout : " + token_timeout);
        }
    }
}
