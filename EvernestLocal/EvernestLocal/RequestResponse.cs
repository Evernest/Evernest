using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class RequestResponse
    {
        public string new_token { get; set; }
        public List<Event> events { get; set; }
        public int token_timeout { get; set; }

        public void ToPrint()
        {
            Console.WriteLine("New_Token : " + new_token + "; ");
            foreach (Event e in events)
            {
                Console.Write("\t"); 
                e.ToPrint();
            }
            Console.WriteLine("  Token_Timeout : " + token_timeout);
        }

    }
}
