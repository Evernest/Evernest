using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class PushEventsResponse
    {
        public string new_token { get; set; }
        public List<int> ids { get; set; }
        public int token_timeout { get; set; }

        public void ToPrint()
        {
            Console.WriteLine("New_Token : " + new_token);
            foreach (int id in ids)
            {
                Console.WriteLine("\t Id : " + id);
            }
            Console.WriteLine("  Token_Timeout : " + token_timeout);
        }
    }
}
