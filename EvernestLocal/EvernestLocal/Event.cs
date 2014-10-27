using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class Event
    {
        public int id { get; set; }
        public string content { get; set; }

        public Event()
        {
            
        }

        public Event(string content)
        {
            this.content = content;
            id = -1;
        }

        public void ToPrint()
        {
            Console.WriteLine("Id : " + id + "; Content : " + content);
        }
    }
}
