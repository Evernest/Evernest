using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class Request
    {
        public string token { get; set; }
        public List<Event> events { get; set; }

        public Request(List<Event> events, String token)
        {
            this.token = token;
            this.events = events;
        }

        public void ToPrint()
        {
            Console.WriteLine("Token : " + token);
            foreach (Event e in events)
            {
                Console.Write("\t");
                e.ToPrint();
            }
        }
    }
}
